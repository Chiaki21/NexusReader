using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexusReader.Shared.Models;

namespace NexusReader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }

       [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel model)
{
    // 1. Find the user by Email
    var user = await _userManager.FindByEmailAsync(model.Email);
    
    // 2. If not found, try finding by UserName (Identity often uses Email as UserName)
    if (user == null)
    {
        user = await _userManager.FindByNameAsync(model.Email);
    }

    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
    {
        return Ok(new { Message = "Login successful" });
    }
    
    return Unauthorized("Invalid login attempt.");
}
    }
}