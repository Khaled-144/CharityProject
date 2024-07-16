namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SalaryHistory
    {
        [Key]
        public int salaries_history_id { get; set; }
        public int emp_id { get; set; }
        public float base_salary { get; set; }
        public float? housing_allowances { get; set; }
        public float? transportaion_allowances { get; set; }
        public float? other_allowances { get; set; }
        public float? overtime { get; set; }
        public float? bonus { get; set; }
        public float? delay_discount { get; set; }
        public float? absence_discount { get; set; }
        public float? other_discount { get; set; }
        public float? debt { get; set; }
        public float? shared_portion { get; set; }
        public float? facility_portion { get; set; }
        public int work_days { get; set; }
        public DateTime date { get; set; }
        public string exchange_statement { get; set; }
        public string? notes { get; set; }
    }

}
