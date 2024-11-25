using LetterBoxd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetterBoxd.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]SignUpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { Errors = errorMessages });
                }

                var sameName = await _userManager.FindByNameAsync(model.Username);
                if (sameName != null)
                {
                    return Conflict(new { Message = "This username is already taken" });
                }

                var newUser = new User
                {
                    UserName = model.Username
                };

                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new { Message = "User creation failed", Errors = result.Errors.Select(e => e.Description) });
                }

                return Ok(new { Message = "User created successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                return StatusCode(500, new { Message = "An error occurred during sign-up", Error = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody]LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { Errors = errorMessages });
                }

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { Message = "Incorrect username or password." });
                }

                return Ok(new { Message = "Login successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }


    }
} 