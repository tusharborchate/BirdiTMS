using BirdiTMS.Extensions;
using BirdiTMS.Middlewares;
using BirdiTMS.Models.Entities;
using BirdiTMS.Models.ViewModels.FromClient;
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


        public BirdiTasksController(UserManager<ApplicationUser> userManager,
            IBirdiTask birdiTaskService
            )
        {
            _userManager = userManager;
            _birdiTaskService = birdiTaskService;

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
            await _birdiTaskService.UpdateTask(clBirdiTask);
            return NoContent();
        }

        // POST: api/BirdiTasks
        [HttpPost]
        public async Task<IActionResult> PostBirdiTask([FromBody] ClBirdiTask clBirdiTask)
        {
            var user = await _userManager.GetUser(User);
            var entity = await _birdiTaskService.CreateTask(clBirdiTask, user);
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
            return NoContent();
        }
    }
}
