using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BirdiTMS.Context;
using BirdiTMS.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using BirdiTMS.Models.ViewModels.FromServer;
using BirdiTMS.Models.ViewModels.FromClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using BirdiTMS.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;
using System.Diagnostics;
using BirdiTMS.Extensions;

namespace BirdiTMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BirdiTasksController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUser _userService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IHttpContextAccessor _userContext;
        private readonly IBirdiTask _birdiTaskService;


        public BirdiTasksController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IUser userService,
            IMapper mapper,
            ApplicationDbContext context,
            ILogger<UsersController> logger,
            IHttpContextAccessor userContext,
            IBirdiTask birdiTaskService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userService = userService;
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _userContext = userContext;
            _birdiTaskService = birdiTaskService;

        }
        // GET: api/BirdiTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BirdiTask>>> GetBirdiTasks()
        {
            var user = await _userManager.GetUser(User);
            var result= await _birdiTaskService.GetAll(user.Id);
            if(result==null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/BirdiTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BirdiTask>> GetBirdiTask(int id)
        {
            var user = await _userManager.GetUser(User);
            var entity = await _birdiTaskService.GetTask(id, user.Id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        // PUT: api/BirdiTasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBirdiTask([FromRoute]int id,[FromBody] ClBirdiTask clBirdiTask)
        {
            var user = await _userManager.GetUser(User);

            if (id != clBirdiTask.Id  )
            {
                return BadRequest();
            }
            if(clBirdiTask.UserId != user.Id)
            {
                return Unauthorized();
            }
            var birdiTask = await _birdiTaskService.GetTask(id, clBirdiTask.UserId);
            if (birdiTask == null)
            {
                return NotFound();
            }
            await _birdiTaskService.UpdateTask(clBirdiTask,user);
            return NoContent();
        }

        // POST: api/BirdiTasks
        [HttpPost]
        public async Task<ActionResult<BirdiTask>> PostBirdiTask([FromBody] ClBirdiTask clBirdiTask)
        {
            var user = await _userManager.GetUser(User);
            var entity = await _birdiTaskService.CreateTask(clBirdiTask, user);
            return CreatedAtAction("GetBirdiTask", new { id = entity.Id }, entity);
        }

        // DELETE: api/BirdiTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBirdiTask(int id)
        {
            var user = await _userManager.GetUser(User);
            var birdiTask = await GetById(id, user.Id);
            if(birdiTask.UserId != user.Id)
            {
                return Unauthorized();
            }
            if (birdiTask == null)
            {
                return NotFound();
            }
            var result = await _birdiTaskService.DeleteTask(id, user.Id);
            if(!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        private async Task<BirdiTask> GetById(int id, string userId)
        {
            return await _context.BirdiTasks.CheckExtension(a => a.Id == id && a.UserId == userId);
        }



    }
}
