using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BirdiTMS.Models;
using BirdiTMS.Models.Entities;
using BirdiTMS.Services;

namespace BirdiTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;    
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUser _userService;

        public UsersController( UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IUser userService) { 
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager; 
            _configuration = configuration;
            _userService = userService;

        }

        

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Username);
            if (user != null)
            {
                var isAuthenticated = await _userManager.CheckPasswordAsync(user, loginUser.Password);
                if (isAuthenticated)
                {
                    var token = await SetTokenWithRefreshToken(user);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
            }
            return Unauthorized();
        }

        [NonAction]
        private async Task<JwtSecurityToken> SetTokenWithRefreshToken(IdentityUser user)
        {
            var token = await _userService.GetJwtSecurityToken(user);
            CookieOptions cookieOptions =new CookieOptions { HttpOnly = true,Expires= DateTime.Now.AddMinutes(2) };
            Response.Cookies.Append("resfreshToken","",cookieOptions);
            return token;
        }
    }
}
