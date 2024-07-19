using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using CharityProject.Models;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;
using IronPdf;
using IronPdf.Rendering;
using IronPdf.Font;
using Microsoft.Extensions.Logging;
using System.Drawing;


namespace CharityProject.Controllers
{
    public class OtherServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OtherServicesController> _logger;


        public OtherServicesController(ApplicationDbContext context, ILogger<OtherServicesController> logger)
        {
            _context = context;
            _logger = logger;
            // Register Bouncy Castle provider
            //Security.AddProvider(new Org.BouncyCastle.Crypto.Providers.BouncyCastleProvider());

        }

        [HttpPost]
        public async Task<IActionResult> GenerateSalaryStatement(string employeeName, string employeeId, string salaryAmount, string recipient, string customRecipientName)
        {
            string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFileName = $"SalaryStatement_{employeeId}_{dateTime}.pdf";
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "pdfs", "Outputs", outputFileName);

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "pdfs", "Templates", "SalaryStatementTemplate.pdf");

            try
            {
                // Load the existing PDF template
                var templatePdf = PdfDocument.FromFile(templatePath);

                // HTML content with placeholders for dynamic data
                string htmlContent = $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: 'Cairo', sans-serif; direction: rtl; }}
                            .field {{ position: absolute; }}
                            #employeeName {{ top: 300px; left: 200px; }}
                            #employeeId {{ top: 350px; left: 200px; }}
                            #salaryAmount {{ top: 400px; left: 200px; }}
                            #recipient {{ top: 450px; left: 200px; }}
                            #customRecipientName {{ top: 500px; left: 200px; }}
                        </style>
                    </head>
                    <body>
                        <div class='field' id='employeeName'>اسم الموظف: {employeeName}</div>
                        <div class='field' id='employeeId'>رقم الموظف: {employeeId}</div>
                        <div class='field' id='salaryAmount'>مبلغ الراتب: {salaryAmount}</div>
                        <div class='field' id='recipient'>إلى: {recipient}</div>
                        {(string.IsNullOrEmpty(customRecipientName) ? "" : $"<div class='field' id='customRecipientName'>اسم الجهة: {customRecipientName}</div>")}
                    </body>
                </html>";

                // Render HTML content to an overlay PDF document
                var Renderer = new ChromePdfRenderer();
                var overlayPdf = Renderer.RenderHtmlAsPdf(htmlContent);

                // Merge the overlay PDF with the template PDF
                var mergedPdf = PdfDocument.Merge(new List<PdfDocument> { templatePdf, overlayPdf });

                // Save the merged PDF document
                mergedPdf.SaveAs(outputPath);

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
                return File(fileBytes, "application/pdf", outputFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF");
                return StatusCode(500, "Error generating PDF");
            }
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
                .FirstOrDefaultAsync(m => m.service_id == id);
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
        public async Task<IActionResult> Create([Bind("service_id,service_name")] OtherService otherService)
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
        public async Task<IActionResult> Edit(int id, [Bind("service_id,service_name")] OtherService otherService)
        {
            if (id != otherService.service_id)
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
                    if (!OtherServiceExists(otherService.service_id))
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
                .FirstOrDefaultAsync(m => m.service_id == id);
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
            return _context.OtherServices.Any(e => e.service_id == id);
        }

    }
}
