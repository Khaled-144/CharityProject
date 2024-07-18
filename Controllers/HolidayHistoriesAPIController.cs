using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using CharityProject.Models;

namespace CharityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayHistoriesAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HolidayHistoriesAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HolidayHistoriesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayHistory>>> GetHolidayHistories()
        {
            return await _context.HolidayHistories.ToListAsync();
        }

        // GET: api/HolidayHistoriesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayHistory>> GetHolidayHistory(int id)
        {
            var holidayHistory = await _context.HolidayHistories.FindAsync(id);

            if (holidayHistory == null)
            {
                return NotFound();
            }

            return holidayHistory;
        }

        // PUT: api/HolidayHistoriesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHolidayHistory(int id, HolidayHistory holidayHistory)
        {
            if (id != holidayHistory.holidays_history_id)
            {
                return BadRequest();
            }

            _context.Entry(holidayHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HolidayHistoryExists(id))
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

        // POST: api/HolidayHistoriesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HolidayHistory>> PostHolidayHistory(HolidayHistory holidayHistory)
        {
            _context.HolidayHistories.Add(holidayHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHolidayHistory", new { id = holidayHistory.holidays_history_id }, holidayHistory);
        }

        // DELETE: api/HolidayHistoriesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHolidayHistory(int id)
        {
            var holidayHistory = await _context.HolidayHistories.FindAsync(id);
            if (holidayHistory == null)
            {
                return NotFound();
            }

            _context.HolidayHistories.Remove(holidayHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HolidayHistoryExists(int id)
        {
            return _context.HolidayHistories.Any(e => e.holidays_history_id == id);
        }
    }
}
