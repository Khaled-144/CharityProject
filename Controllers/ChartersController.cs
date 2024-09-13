using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using CharityProject.Models;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;

namespace CharityProject.Controllers
{
    public class ChartersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartersController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Charter()
        {

            return View();
        }

        // GET: Charters
        public async Task<IActionResult> Index()
        {
            return View(await _context.charter.ToListAsync());
        }

        // GET: Charters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charter = await _context.charter
                .FirstOrDefaultAsync(m => m.charter_id == id);
            if (charter == null)
            {
                return NotFound();
            }

            return View(charter);
        }



        // GET: Charters/Create
        /*   public IActionResult Create1()
            {

                ViewData["Departments"] = _context.Department.Select(d => new
                {
                        d.departement_name,
                            d.departement_id
                }).ToList();

                ViewData["Employees"] = _context.employee.Select(e => new
                {
                    e.employee_id,
                    e.name
                }).ToList();

                return View();
            }*/


        public IActionResult Create1()
        {
            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(),
                Text = d.departement_name
            }).ToList();

            ViewData["Employees"] = _context.employee.Select(e => new SelectListItem
            {
                Value = e.employee_id.ToString(),
                Text = e.name
            }).ToList();

            return View();
        }

        // POST: Charters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create1([Bind("charter_id,charter_info,serial_number,creation_date,from_departement_name,status,notes,to_departement_name,to_emp_id,receive_date,end_date")] charter charter)
        {



            _context.Add(charter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));







        }


        // GET: Charters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charter = await _context.charter.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("charter_id,charter_info,serial_number,creation_date,from_departement_name,status,notes,to_departement_name,to_emp_id,receive_date,end_date")] charter charter)
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

            var charter = await _context.charter
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
            var charter = await _context.charter.FindAsync(id);
            if (charter != null)
            {
                _context.charter.Remove(charter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharterExists(int id)
        {
            return _context.charter.Any(e => e.charter_id == id);
        }







        /// <summary>
        /// //////////////////
        /// </summary>
        /// <returns></returns>
        /// 




        //////////////////////






        /*
                public IActionResult Create_Charter()
                {
                    return View();
                }
        */
        // POST: Charters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*           [HttpPost]
                   [ValidateAntiForgeryToken]
                   public async Task<IActionResult> Create_Charter([Bind("charter_info,serial_number,from_departement_name,status,notes,to_departement_name,to_emp_id,receive_date,end_date")] charter charter)
                   {
                       if (ModelState.IsValid)
                       {

                      charter.creation_date = DateOnly.FromDateTime(DateTime.Now);
                      _context.Add(charter);
                           await _context.SaveChangesAsync();
                           return RedirectToAction(nameof(Index));
                       }
                       return View(charter);
                   }*/
        public async Task<IActionResult> UpdateStatus(int charter_id)
        {
            var charter = await _context.charter.FindAsync(charter_id);
            if (charter == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            charter.status = "مستلمة";
            charter.receive_date = DateTime.Now;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}
