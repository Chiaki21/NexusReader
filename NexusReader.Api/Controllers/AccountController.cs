using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NexusReader.Shared.Models;

namespace NexusReader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public AccountController(UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _userManager = userManager;
            _cache = cache;
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] string email) // Changed to async Task
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is required.");

            try 
            {
                // 1. Generate a random 6-digit code
                var code = new Random().Next(100000, 999999).ToString();

                // 2. Store in cache for 15 minutes
                _cache.Set(email, code, TimeSpan.FromMinutes(15));

                // 3. TRIGGER THE ACTUAL EMAIL
                // We create an instance of your EmailController to use its logic
                var emailController = new EmailController();
                var emailRequest = new EmailRequest { Email = email, Code = code };
                
                var emailResult = await emailController.SendEmail(emailRequest);

                // If EmailController returns a 500, we catch it here
                if (emailResult is ObjectResult obj && obj.StatusCode == 500)
                {
                    return StatusCode(500, obj.Value);
                }

                // Debug fallback in case email is slow
                Console.WriteLine($"***********************************************");
                Console.WriteLine($"SENT TO {email}: {code}");
                Console.WriteLine($"***********************************************");

                return Ok(new { Message = "Verification code sent! Please check your inbox." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // 1. Check the Cache for the code
            if (!_cache.TryGetValue(model.Email, out string? storedCode))
            {
                return BadRequest(new { Message = "Code expired. Please request a new one." });
            }

            if (storedCode != model.VerificationCode)
            {
                return BadRequest(new { Message = "Invalid verification code." });
            }

            // 2. Code is correct, now create the user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsOver18 = model.IsOver18
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _cache.Remove(model.Email); // Cleanup cache on success
                return Ok(new { Message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Ok(new { Message = "Login successful", FirstName = user.FirstName });
            }
            return Unauthorized("Invalid login attempt.");
        }
    }
}