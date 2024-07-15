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
    public class OtherServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OtherServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OtherServices
        public async Task<IActionResult> Index()
        {
            return View(await _context.OtherServices.ToListAsync());
        }

        // GET: OtherServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otherService = await _context.OtherServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (otherService == null)
            {
                return NotFound();
            }

            return View(otherService);
        }

        // GET: OtherServices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OtherServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceName,Description")] OtherService otherService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(otherService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(otherService);
        }

        // GET: OtherServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otherService = await _context.OtherServices.FindAsync(id);
            if (otherService == null)
            {
                return NotFound();
            }
            return View(otherService);
        }

        // POST: OtherServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceName,Description")] OtherService otherService)
        {
            if (id != otherService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(otherService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OtherServiceExists(otherService.Id))
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
            return View(otherService);
        }

        // GET: OtherServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otherService = await _context.OtherServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (otherService == null)
            {
                return NotFound();
            }

            return View(otherService);
        }

        // POST: OtherServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var otherService = await _context.OtherServices.FindAsync(id);
            if (otherService != null)
            {
                _context.OtherServices.Remove(otherService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OtherServiceExists(int id)
        {
            return _context.OtherServices.Any(e => e.Id == id);
        }
    }
}
