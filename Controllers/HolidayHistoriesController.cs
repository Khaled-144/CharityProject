using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using CharityProject.Models;

namespace CharityProject.Controllers
{
    public class HolidayHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HolidayHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HolidayHistories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HolidayHistories.Include(h => h.holiday);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HolidayHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidayHistory = await _context.HolidayHistories
                .Include(h => h.holiday)
                .FirstOrDefaultAsync(m => m.holidays_history_id == id);
            if (holidayHistory == null)
            {
                return NotFound();
            }

            return View(holidayHistory);
        }
        public async Task<IActionResult> Holidays()
        {
            var applicationDbContext = _context.HolidayHistories.Include(h => h.holiday);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HolidayHistories/Create
        public IActionResult Create()
        {
            ViewData["holiday_id"] = new SelectList(_context.Holidays, "holiday_id", "holiday_id");
            return View();
        }

        // POST: HolidayHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("holidays_history_id,holiday_id,duration,emp_id,start_date,end_date,status")] HolidayHistory holidayHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(holidayHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["holiday_id"] = new SelectList(_context.Holidays, "holiday_id", "holiday_id", holidayHistory.holiday_id);
            return View(holidayHistory);
        }

        // GET: HolidayHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidayHistory = await _context.HolidayHistories.FindAsync(id);
            if (holidayHistory == null)
            {
                return NotFound();
            }
            ViewData["holiday_id"] = new SelectList(_context.Holidays, "holiday_id", "holiday_id", holidayHistory.holiday_id);
            return View(holidayHistory);
        }

        // POST: HolidayHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("holidays_history_id,holiday_id,duration,emp_id,start_date,end_date,status")] HolidayHistory holidayHistory)
        {
            if (id != holidayHistory.holidays_history_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(holidayHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HolidayHistoryExists(holidayHistory.holidays_history_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["holiday_id"] = new SelectList(_context.Holidays, "holiday_id", "holiday_id", holidayHistory.holiday_id);
            return View(holidayHistory);
        }

        // GET: HolidayHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidayHistory = await _context.HolidayHistories
                .Include(h => h.holiday)
                .FirstOrDefaultAsync(m => m.holidays_history_id == id);
            if (holidayHistory == null)
            {
                return NotFound();
            }

            return View(holidayHistory);
        }

        // POST: HolidayHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var holidayHistory = await _context.HolidayHistories.FindAsync(id);
            if (holidayHistory != null)
            {
                _context.HolidayHistories.Remove(holidayHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HolidayHistoryExists(int id)
        {
            return _context.HolidayHistories.Any(e => e.holidays_history_id == id);
        }
    }
}
