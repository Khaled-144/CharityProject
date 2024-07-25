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
    public class ChartersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Charters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Charter.ToListAsync());
        }

        // GET: Charters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charter = await _context.Charter
                .FirstOrDefaultAsync(m => m.charter_id == id);
            if (charter == null)
            {
                return NotFound();
            }

            return View(charter);
        }

        // GET: Charters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Charters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("charter_id,charter_info,serial_number,creation_date,from_departement_name,status,notes,to_departement_name,to_emp_id,receive_date,end_date")] Charter charter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(charter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(charter);
        }

        // GET: Charters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charter = await _context.Charter.FindAsync(id);
            if (charter == null)
            {
                return NotFound();
            }
            return View(charter);
        }

        // POST: Charters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("charter_id,charter_info,serial_number,creation_date,from_departement_name,status,notes,to_departement_name,to_emp_id,receive_date,end_date")] Charter charter)
        {
            if (id != charter.charter_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharterExists(charter.charter_id))
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
            return View(charter);
        }

        // GET: Charters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charter = await _context.Charter
                .FirstOrDefaultAsync(m => m.charter_id == id);
            if (charter == null)
            {
                return NotFound();
            }

            return View(charter);
        }

        // POST: Charters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charter = await _context.Charter.FindAsync(id);
            if (charter != null)
            {
                _context.Charter.Remove(charter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharterExists(int id)
        {
            return _context.Charter.Any(e => e.charter_id == id);
        }
    }
}
