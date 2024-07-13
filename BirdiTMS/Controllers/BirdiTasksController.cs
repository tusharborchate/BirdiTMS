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

namespace BirdiTMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BirdiTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BirdiTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BirdiTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BirdiTask>>> GetBirdiTasks()
        {
            return await _context.BirdiTasks.ToListAsync();
        }

        // GET: api/BirdiTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BirdiTask>> GetBirdiTask(int id)
        {
            var birdiTask = await _context.BirdiTasks.FindAsync(id);

            if (birdiTask == null)
            {
                return NotFound();
            }

            return birdiTask;
        }

        // PUT: api/BirdiTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBirdiTask(int id, BirdiTask birdiTask)
        {
            if (id != birdiTask.Id)
            {
                return BadRequest();
            }

            _context.Entry(birdiTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BirdiTaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BirdiTasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BirdiTask>> PostBirdiTask(BirdiTask birdiTask)
        {
            _context.BirdiTasks.Add(birdiTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBirdiTask", new { id = birdiTask.Id }, birdiTask);
        }

        // DELETE: api/BirdiTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBirdiTask(int id)
        {
            var birdiTask = await _context.BirdiTasks.FindAsync(id);
            if (birdiTask == null)
            {
                return NotFound();
            }

            _context.BirdiTasks.Remove(birdiTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BirdiTaskExists(int id)
        {
            return _context.BirdiTasks.Any(e => e.Id == id);
        }
    }
}
