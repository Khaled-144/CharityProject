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
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Elements;


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

        private async Task<employee_details> GetEmployeeDetailsFromSessionAsync()
        {
            var employeeIdString = HttpContext.Session.GetString("Id");

            if (employeeIdString != null && int.TryParse(employeeIdString, out int employeeId))
            {
                var employeeDetails = await _context.employee_details
                    .Include(ed => ed.employee)
                   .Include(ed => ed.Department)
                    .FirstOrDefaultAsync(ed => ed.employee_id == employeeId);

                return employeeDetails;
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSalaryStatement(string recipient)
        {
            var employee = await GetEmployeeDetailsFromSessionAsync(); // Retrieve employee details from session
            string employeeName = employee.employee.name;
            string employeeId = employee.employee_id.ToString();
            QuestPDF.Settings.License = LicenseType.Community;
            string dateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            string outputFileName = $"تعريف بالراتب للموظف_{employeeName}_بتاريخ_{dateTime}.pdf";

            try
            {
                // Fetch the latest salary record based on the employee ID
                var salaryHistory = await GetLastEmployeeSalaryAsync(employee.employee_id);
                if (salaryHistory == null)
                {
                    return StatusCode(404, "لايوجد رواتب مسجلة لهذا الموظف");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(container => ComposeContent(container, employeeName, employeeId, salaryHistory, recipient));
                        page.Footer().Element(ComposeFooter);

                        page.Background().Border(1).BorderColor(Colors.Grey.Lighten2);
                    });
                });

                byte[] pdfBytes;
                using (var ms = new MemoryStream())
                {
                    document.GeneratePdf(ms);
                    pdfBytes = ms.ToArray();
                }

                // Optionally save the file on the server
                string savePath = Path.Combine("pdfs", "outputs", outputFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                await System.IO.File.WriteAllBytesAsync(savePath, pdfBytes);

                // Return the file to the client
                return File(pdfBytes, "application/pdf", outputFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Error generating PDF");
            }
        }

        // Method to get the latest salary record for an employee
        private async Task<salaries_history> GetLastEmployeeSalaryAsync(int employeeId)
        {

                 return await _context.SalaryHistories
                           .Where(s => s.emp_id == employeeId)
                           .OrderByDescending(s => s.date) // Retrieve the latest salary based on date
                           .FirstOrDefaultAsync();
            
        }

        void ComposeHeader(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Padding(20).Row(row =>
            {
                row.ConstantItem(100).Height(50).Image("wwwroot/images/logo.png");
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text("تعريف بالراتب").FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                    column.Item().AlignCenter().Text($"التاريخ: {GetArabicFormattedDate(DateTime.Now)}").FontSize(12).FontColor(Colors.Grey.Medium);
                });
            });
        }

        string GetArabicFormattedDate(DateTime date)
        {
            string[] arabicMonths = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            string day = new string(date.Day.ToString("00").Reverse().ToArray());
            string year = new string(date.Year.ToString().Reverse().ToArray());
            return $"{day} {arabicMonths[date.Month - 1]} {year}";
        }

        void ComposeContent(IContainer container, string employeeName, string employeeId, salaries_history salaryHistory, string recipient)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(20);

                column.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(innerColumn =>
                {
                    innerColumn.Item().AlignRight().Text("نموذج تعريف بالراتب لموظفين جمعية مسكني – المدينة المنورة").Bold().FontSize(14);
                    innerColumn.Item().AlignRight().Text("هذا النموذج خاص بجمعية مسكني وتم انشاء هذه الصفحة بناءً على طلب الموظف ولا تتحمل الجمعية أي مسؤولية بخصوص صحة البيانات أدناه.").FontSize(10);
                });

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();  // Label column
                        columns.ConstantColumn(150); // Value column
                    });

                    // Format the salary amount as a string with 2 decimal places
                    string salaryText = salaryHistory.base_salary.ToString("N2");
                    string reversedSalaryText = new string(salaryText.Reverse().ToArray());

                    // Add table rows
                    AddTableRow(table, "اسم الموظف:", employeeName);
                    AddTableRow(table, "رقم الموظف:", employeeId);
                    AddTableRow(table, "مبلغ الراتب:", $"{reversedSalaryText} ريال سعودي فقط");
                    AddTableRow(table, "الى:", recipient);
                });

                column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                column.Item().PaddingTop(20).AlignRight().Text("هذا المستند يوثق معلومات الراتب").FontSize(10).FontColor(Colors.Grey.Medium);
            });
        }

        // Helper method to add table rows
        void AddTableRow(TableDescriptor table, string label, string value)
        {
            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(value);
            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(label).Bold();
        }

        void ComposeFooter(IContainer container)
        {
            string reversedYear = new string(DateTime.Now.Year.ToString().Reverse().ToArray());
            container.Background(Colors.Grey.Lighten3).Padding(10).Row(row =>
            {
                row.RelativeItem().AlignRight().Text($"© {reversedYear} جمعية مسكني").FontSize(10).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignLeft().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }
        //--------------salar recored----------------
        [HttpPost]
        public async Task<IActionResult> GenerateSalaryRecored(DateTime startDate, DateTime endDate)
        {
            var employee = await GetEmployeeDetailsFromSessionAsync();
            string employeeName = employee.employee.name;
            int employeeId = employee.employee_id;
            QuestPDF.Settings.License = LicenseType.Community;
            string dateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            string outputFileName = $"تقرير بالراتب للموظف_{employeeName}_بتاريخ_{dateTime}.pdf";

            try
            {
                var salaryRecords = await GetSalaryRecordsAsync(employeeId, startDate, endDate);
                if (!salaryRecords.Any())  // Use .Any() to check if there are any records
                {
                    return StatusCode(404, "لايوجد رواتب مسجلة لهذا الموظف في هذه الفترة");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                        page.Header().Element(ComposeHeaderRecored);
                        page.Content().Element(container => ComposeContent(container, employeeName, employeeId.ToString(), startDate, endDate, salaryRecords));
                        page.Footer().Element(ComposeFooter);

                        page.Background().Border(1).BorderColor(Colors.Grey.Lighten2);
                    });
                });

                byte[] pdfBytes;
                using (var ms = new MemoryStream())
                {
                    document.GeneratePdf(ms);
                    pdfBytes = ms.ToArray();
                }

                string savePath = Path.Combine("pdfs", "outputs", outputFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                await System.IO.File.WriteAllBytesAsync(savePath, pdfBytes);

                return File(pdfBytes, "application/pdf", outputFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Error generating PDF");
            }
        }

        private async Task<List<salaries_history>> GetSalaryRecordsAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            return await _context.SalaryHistories
                .Where(r => r.emp_id == employeeId && r.date >= startDate && r.date <= endDate)
                .OrderBy(r => r.date)
                .ToListAsync();
        }
        void ComposeHeaderRecored(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Padding(20).Row(row =>
            {
                row.ConstantItem(100).Height(50).Image("wwwroot/images/logo.png");
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text("تقرير الراتب").FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                    column.Item().AlignCenter().Text($"التاريخ: {GetArabicFormattedDate(DateTime.Now)}").FontSize(12).FontColor(Colors.Grey.Medium);
                });
            });
        }
        private void ComposeContent(IContainer container, string employeeName, string employeeId, DateTime startDate, DateTime endDate, List<salaries_history> salaryRecords)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(10);


                column.Item().AlignRight().Text($"اسم الموظف: {employeeName}").SemiBold();
                column.Item().AlignRight().Text($"رقم الموظف: {ReverseNumber(employeeId)}");
                column.Item().AlignRight().Text($"الفترة: {GetArabicMonthYear(endDate)} - {GetArabicMonthYear(startDate)}");

                column.Item().AlignCenter().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(100); // الإجمالي النهائي
                        columns.ConstantColumn(100); // إجمالي الخصومات
                        columns.ConstantColumn(100); // إجمالي البدلات
                        columns.ConstantColumn(80);  // التاريخ
                    });

                    table.Header(header =>
                    {
                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text("الإجمالي النهائي").SemiBold();
                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text("إجمالي الخصومات").SemiBold();
                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text("إجمالي البدلات").SemiBold();
                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text("التاريخ").SemiBold();
                    });

                    foreach (var record in salaryRecords.OrderByDescending(r => r.date))
                    {
                        decimal totalAllowances = (decimal)record.base_salary
                            + (decimal)(record.housing_allowances ?? 0)
                            + (decimal)(record.transportaion_allowances ?? 0)
                            + (decimal)(record.other_allowances ?? 0)
                            + (decimal)(record.overtime ?? 0)
                            + (decimal)(record.bonus ?? 0);

                        decimal totalDiscounts = (decimal)(record.delay_discount ?? 0)
                            + (decimal)(record.absence_discount ?? 0)
                            + (decimal)(record.other_discount ?? 0)
                            + (decimal)(record.debt ?? 0)
                            + (decimal)(record.shared_portion ?? 0);
                           

                        decimal finalTotal = totalAllowances - totalDiscounts;

                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignRight().Text(finalTotal.ToString("N2"));
                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignRight().Text(totalDiscounts.ToString("N2"));
                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignRight().Text(totalAllowances.ToString("N2"));
                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignRight().Text(GetArabicMonthYear(record.date));
                    }
                });
            });
        }
        private string ReverseNumber(string number)
        {
            return new string(number.Reverse().ToArray());
        }
        private string GetArabicMonthYear(DateTime date)
        {
            string[] arabicMonths = new string[] {
        "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
        "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
    };

            string reversedYear = new string(date.Year.ToString().Reverse().ToArray());
            return $"{arabicMonths[date.Month - 1]} {reversedYear}";
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
        public IActionResult index()
        {
            return View();
        }
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
