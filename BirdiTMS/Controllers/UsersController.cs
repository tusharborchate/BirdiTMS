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
using BirdiTMS.Services;
using Azure;
using BirdiTMS.Models.ViewModels.FromClient;
using AutoMapper;
using BirdiTMS.Models.ViewModels.FromServer;
using BirdiTMS.Context;
using BirdiTMS.Models.Entities;

namespace BirdiTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;    
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUser _userService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;


        public UsersController( UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IUser userService,
            IMapper mapper,
            ApplicationDbContext context,
            ILogger<UsersController> logger
            ) { 
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userService = userService;
            _mapper = mapper;
            _context = context;
            _logger = logger;

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("getting data");
            var data = _context.Users.AsQueryable();
            return Ok( _mapper.Map<List<SrUserViewModel>>(data.ToList()));
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
                    var token = await SetTokenWithRefreshToken(user);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = DateTime.Now.AddMinutes(1)    
                    });
                }
            }
            return Unauthorized();
        }

        [NonAction]
        private async Task<JwtSecurityToken> SetTokenWithRefreshToken(ApplicationUser user)
        {
            var token = await _userService.GetJwtSecurityToken(user);
            CookieOptions cookieOptions =new CookieOptions { HttpOnly = true,Expires= DateTime.Now.AddMinutes(2) };
            Response.Cookies.Append("resfreshToken","",cookieOptions);
            return token;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return BadRequest();

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest();
            await SetTokenWithRefreshToken(user);
            return Created();
        }
    }
}
