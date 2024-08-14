using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharityProject.Models
{
    [Table("salaries_history")]
    public class salaries_history
    {
        [Key]
        public int salaries_history_id { get; set; }

        [ForeignKey("employee")]
        public int emp_id { get; set;}

        public Double base_salary { get; set; }
        public Double? housing_allowances { get; set; }
        public Double? transportaion_allowances { get; set; }
        public Double? other_allowances { get; set; }
        public Double? overtime { get; set; }
        public Double? bonus { get; set; }
        public Double? delay_discount { get; set; }
        public Double? absence_discount { get; set; }
        public Double? other_discount { get; set; }
        public Double? debt { get; set; }
        public Double? shared_portion { get; set; }
        public Double? facility_portion { get; set; }
        public Double? Social_insurance { get; set; }
        public int work_days { get; set; }
        public DateTime date { get; set; }
        public string exchange_statement { get; set; }
        public string? notes { get; set; }

        // Navigation property
        public virtual employee employee { get; set; }
    }
}