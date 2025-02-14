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

    [PermissionFilter("المدير التنفيذي")]

    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Department
                .Include(d => d.Supervisor)
                .ToListAsync();

            ViewBag.Employees = await _context.employee.ToListAsync();

            return View(departments);
        }


        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .FirstOrDefaultAsync(m => m.departement_id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("departement_id,departement_name,supervisor_id")] Department department)
        {

            _context.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            return View(department);
        }

        // GET: Departments/Edit/5
        /*  public async Task<IActionResult> Edit(int? id)
   {
       if (id == null)
       {
           return NotFound();
       }

       // Fetch the department with its supervisor included
       var department = await _context.Department
           .Include(d => d.Supervisor)  // Include the Supervisor navigation property
           .Where(d => d.departement_id == id)
           .FirstOrDefaultAsync();
               ViewBag.Employees = await _context.employee.ToListAsync();
               ViewBag.CurrentSupervisorName = department.Supervisor?.name ?? "No supervisor assigned";

               if (department == null)
       {
           return NotFound();
       }

       // Populate the list of employees for the dropdown
       ViewBag.Employees = await _context.employee.ToListAsync();

       // Return the department model to the view
       return View(department);
   }*/





        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("departement_id,departement_name,supervisor_id,permission_position")] Department department)
        {
            if (id != department.departement_id)
            {
                return NotFound();
            }

            try
            {
                // Retrieve the current department with the existing supervisor
                var existingDepartment = await _context.Department
                    .Include(d => d.Supervisor)
                    .FirstOrDefaultAsync(d => d.departement_id == id);

                if (existingDepartment == null)
                {
                    return NotFound();
                }

                string previousDepartmentMessage = null;
                bool wasPreviouslyManager = false;

                // Update the old supervisor's details
                if (existingDepartment.supervisor_id != null && existingDepartment.supervisor_id != department.supervisor_id)
                {
                    var oldSupervisor = await _context.employee_details
                        .FirstOrDefaultAsync(e => e.employee_id == existingDepartment.supervisor_id);
                    if (oldSupervisor != null)
                    {
                        oldSupervisor.position = "موظف"; // Set old supervisor's position to 'Employee'
                        oldSupervisor.permission_position = "موظف"; // Reset permission_position
                        _context.Update(oldSupervisor);
                    }
                }

                // Check if the new supervisor is already a manager in another department
                if (department.supervisor_id != null)
                {
                    var existingManagerDepartment = await _context.Department
                        .FirstOrDefaultAsync(d => d.supervisor_id == department.supervisor_id && d.departement_id != department.departement_id);
                    if (existingManagerDepartment != null)
                    {
                        wasPreviouslyManager = true;
                        // Update the old manager's details
                        var oldManager = await _context.employee_details
                            .FirstOrDefaultAsync(e => e.employee_id == department.supervisor_id);
                        if (oldManager != null)
                        {
                            oldManager.position = "موظف"; // Set old manager's position to 'Employee'
                            oldManager.permission_position = "موظف"; // Reset permission_position
                            _context.Update(oldManager);
                        }
                        // Clear the old manager's supervisor_id from their previous department
                        existingManagerDepartment.supervisor_id = null;
                        _context.Update(existingManagerDepartment);
                        previousDepartmentMessage = "القسم الذي كان يديره هذا المدير الآن شاغر بلا مدير.";
                    }
                }

                // Update the new manager's details
                var newManager = await _context.employee_details
                    .FirstOrDefaultAsync(e => e.employee_id == department.supervisor_id);
                if (newManager != null)
                {
                    if (department.departement_name != "الادارة التنفيذية")
                    {
                        newManager.position = $"مدير {department.departement_name}"; // Set new manager's position
                        newManager.permission_position = $"مدير {department.departement_name}"; // Update new manager's permission_position
                        newManager.departement_id = department.departement_id; // Set department_id for new manager
                    }
                    else
                    {
                        newManager.position = "المدير التنفيذي"; // Set new manager's position
                        newManager.permission_position = "المدير التنفيذي"; ; // Update new manager's permission_position
                        newManager.departement_id = department.departement_id; // Set department_id for new manager
                    }
                    _context.Update(newManager);

                }

                // Update the department's supervisor
                existingDepartment.supervisor_id = department.supervisor_id;
                existingDepartment.departement_name = department.departement_name;
                _context.Update(existingDepartment);

                await _context.SaveChangesAsync();

                // Set TempData message
                if (wasPreviouslyManager)
                {
                    TempData["Message"] = previousDepartmentMessage;
                }
                else
                {
                    TempData["Message"] = "تم تحديث المدير بنجاح!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(department.departement_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .FirstOrDefaultAsync(m => m.departement_id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department != null)
            {
                _context.Department.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.departement_id == id);
        }
    }
}
