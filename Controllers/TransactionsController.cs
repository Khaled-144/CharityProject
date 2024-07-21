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
	public class TransactionsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TransactionsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Transactions
		public async Task<IActionResult> Index()
		{
			return View(await _context.Transactions.ToListAsync());
		}

		public async Task<IActionResult> SearchById(int id, string sortOrder)
		{
			var transactions = from t in _context.Transactions
							   select t;

			if (id != 0)
			{
				transactions = transactions.Where(n => n.transaction_id == id);
			}

			switch (sortOrder)
			{
				case "oldest":
					transactions = transactions.OrderBy(t => t.create_date);
					break;
				case "newest":
					transactions = transactions.OrderByDescending(t => t.create_date);
					break;
				default:
					break;
			}

			return View("Index", await transactions.ToListAsync());
		}





		public IActionResult CreateNewTransaction()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNewTransaction([Bind("close_date,status,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
		{
			if (ModelState.IsValid)
			{
				transaction.create_date = DateTime.Now;
				_context.Add(transaction);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(transaction);
		}

		// GET: Transactions/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions
				.FirstOrDefaultAsync(m => m.transaction_id == id);
			if (transaction == null)
			{
				return NotFound();
			}

			return View(transaction);
		}

		// GET: Transactions/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Transactions/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("close_date,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
		{
			if (ModelState.IsValid)
			{
				transaction.create_date = DateTime.Now;  // Ensure the date is set to the current UTC time
				transaction.status = "Pending";  // Set default status value
				_context.Add(transaction);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(transaction);
		}


		// GET: Transactions/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction == null)
			{
				return NotFound();
			}
			return View(transaction);
		}

		// POST: Transactions/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("transaction_id,type,create_date,close_date,status,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
		{
			if (id != transaction.transaction_id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(transaction);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TransactionExists(transaction.transaction_id))
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
			return View(transaction);
		}

		// GET: Transactions/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions
				.FirstOrDefaultAsync(m => m.transaction_id == id);
			if (transaction == null)
			{
				return NotFound();
			}

			return View(transaction);
		}

		// POST: Transactions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction != null)
			{
				_context.Transactions.Remove(transaction);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool TransactionExists(int id)
		{
			return _context.Transactions.Any(e => e.transaction_id == id);
		}
	}
}
