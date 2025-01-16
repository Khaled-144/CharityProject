using CharityProject.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharityProject.Models
{
    [Table("employee_salary")]
    public class Salary
    {
        [Key]
        [ForeignKey("employee")]
        public int emp_id { get; set; } // Foreign key to the Employee table

        [Required]
        public decimal base_salary { get; set; } // الراتب الأساسي

        public decimal? housing_allowances { get; set; } // بدل السكن
        public decimal? transportation_allowances { get; set; } // بدل المواصلات
        public decimal? other_allowances { get; set; } // بدلات أخرى
        public decimal? shared_portion { get; set; } // حصة المشترك
        public decimal? facility_portion { get; set; } // حصة المنشأة
        public decimal? Social_insurance_rate { get; set; } // نسبة التامينات (e.g., 9.75 or 11.75)
        public decimal? max_overtime_rate { get; set; } // الحد الأقصى لمعدل العمل الإضافي

        [MaxLength(500)]
        public string? salary_notes { get; set; } // ملاحظات اختيارية

        // Navigation property (optional)
    }
}
