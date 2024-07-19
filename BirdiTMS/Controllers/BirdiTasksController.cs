using BirdiTMS.Extensions;
using BirdiTMS.Middlewares;
using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromClient;
using BirdiTMS.Models.ViewModels.FromServer;
using BirdiTMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BirdiTMS.Controllers
{
    [Authorize]
    [AuthFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class BirdiTasksController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBirdiTask _birdiTaskService;
        private readonly ILogger<BirdiTasksController> _logger;

        public BirdiTasksController(UserManager<ApplicationUser> userManager,
            IBirdiTask birdiTaskService,
            ILogger<BirdiTasksController> logger
            )
        {
            _userManager = userManager;
            _birdiTaskService = birdiTaskService;
            _logger = logger;

        }
        // GET: api/BirdiTasks
        [HttpGet]
        public async Task<IActionResult> GetBirdiTasks()
        {
            var user = await _userManager.GetUser(User);
            var result = await _birdiTaskService.GetAll(user.Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/BirdiTasks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBirdiTask(int id)
        {
            var entity = await _birdiTaskService.GetTask(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PUT: api/BirdiTasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBirdiTask([FromRoute] int id, [FromBody] ClBirdiTask clBirdiTask)
        {
            if (id != clBirdiTask.Id)
            {
                return BadRequest();
            }
            // no need of try catch as we are using global exception middleware
            await _birdiTaskService.UpdateTask(clBirdiTask);
            _logger.LogInformation(" task updated " + clBirdiTask.Id);

            return NoContent();
        }

        // POST: api/BirdiTasks
        [HttpPost]
        public async Task<IActionResult> PostBirdiTask([FromBody] ClBirdiTask clBirdiTask)
        {
            var user = await _userManager.GetUser(User);
            var entity = await _birdiTaskService.CreateTask(clBirdiTask, user);
            _logger.LogInformation(" task created " + clBirdiTask.Id);

            return CreatedAtAction("GetBirdiTask", new { id = entity.Id }, entity);
        }

        // DELETE: api/BirdiTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBirdiTask(int id)
        {

            var result = await _birdiTaskService.DeleteTask(id);
            if (!result)
            {
                return BadRequest();
            }
            _logger.LogInformation(" task deleted " + id);

            return NoContent();
        }
    }
}
