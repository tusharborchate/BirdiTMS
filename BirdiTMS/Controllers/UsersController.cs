using AutoMapper;
using BirdiTMS.Context;
using BirdiTMS.Extensions;
using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromClient;
using BirdiTMS.Models.ViewModels.FromServer;
using BirdiTMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BirdiTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;    
        private readonly IUser _userService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;


        public UsersController( UserManager<ApplicationUser> userManager,
            IUser userService,
            IMapper mapper,
            ApplicationDbContext context,
            ILogger<UsersController> logger
            ) { 
            _userManager = userManager;
            _userService = userService;
            _mapper = mapper;
            _context = context;
            _logger = logger;

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _userManager.GetUser(User);
            return Ok( _mapper.Map<SrUserViewModel>(data));
        }
        

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] ClUserViewModel loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user != null)
            {
                var isAuthenticated = await _userManager.CheckPasswordAsync(user, loginUser.Password);
                if (isAuthenticated)
                {
                    _logger.LogInformation(" user authenticated " + user.Email);

                    var token = await SetTokenWithRefreshToken(user);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = DateTime.Now.AddMinutes(3)    
                    });
                }
            }
            return Ok("Invalid Credentials");
        }

        [NonAction]
        private async Task<JwtSecurityToken> SetTokenWithRefreshToken(ApplicationUser user)
        {
            var token = await _userService.GetJwtSecurityToken(user);
            CookieOptions cookieOptions =new CookieOptions { HttpOnly = true,Expires= DateTime.Now.AddMinutes(3) };
            Response.Cookies.Append("resfreshToken","",cookieOptions);
            return token;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return Conflict("User already exist");

            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            _logger.LogInformation("new user created "+ user.Email);

            await SetTokenWithRefreshToken(user);

            return Created("/login",_mapper.Map<SrUserViewModel>(user));
        }
    }
}
