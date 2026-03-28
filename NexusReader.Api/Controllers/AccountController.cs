using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NexusReader.Api.Services;
using NexusReader.Shared.Models;

namespace NexusReader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        private readonly JwtTokenService _jwt;

        public AccountController(UserManager<ApplicationUser> userManager, IMemoryCache cache, JwtTokenService jwt)
        {
            _userManager = userManager;
            _cache = cache;
            _jwt = jwt;
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is required.");

            try
            {
                var code = new Random().Next(100000, 999999).ToString();
                _cache.Set(email, code, TimeSpan.FromMinutes(15));

                var emailController = new EmailController();
                var emailRequest = new EmailRequest { Email = email, Code = code };
                var emailResult = await emailController.SendEmail(emailRequest);

                if (emailResult is ObjectResult obj && obj.StatusCode == 500)
                    return StatusCode(500, obj.Value);

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
            if (!_cache.TryGetValue(model.Email, out string? storedCode))
                return BadRequest(new { Message = "Code expired. Please request a new one." });

            if (storedCode != model.VerificationCode)
                return BadRequest(new { Message = "Invalid verification code." });

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
                _cache.Remove(model.Email);
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
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwt.CreateToken(user, roles);
                return Ok(new
                {
                    Message = "Login successful",
                    Token = token,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = roles
                });
            }
            return Unauthorized("Invalid login attempt.");
        }
    }
}
