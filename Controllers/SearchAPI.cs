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
                .Include(h => h.Holiday)
                .Where(h => h.HolidayHistoryId == parsedQuery ||
                            h.Holiday.Type.Contains(query) ||
                            h.Status.Contains(query) ||
                            h.EmpId.ToString().Contains(query) ||
                            h.Duration.ToString().Contains(query) ||
                            h.StartDate.ToString().Contains(query) ||
                            h.EndDate.ToString().Contains(query))
                .ToListAsync();

            return Ok(results);
        }
    }
}
