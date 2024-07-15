using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchAPI : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SearchAPI (ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<HolidayHistory>>> Search([FromQuery] string query)
        {
            int.TryParse(query, out int parsedQuery);
            var results = await _context.HolidayHistories
                .Include(h => h.holiday)
                .Where(h => h.holidays_history_id == parsedQuery ||
                            h.holiday.type.Contains(query) ||
                            h.status.Contains(query) ||
                            h.emp_id.ToString().Contains(query) ||
                            h.duration.ToString().Contains(query) ||
                            h.start_date.ToString().Contains(query) ||
                            h.end_date.ToString().Contains(query))
                .ToListAsync();

            return Ok(results);
        }
    }
}
