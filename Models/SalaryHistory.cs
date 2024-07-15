namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SalaryHistory
    {
        [Key]
        public int SalaryHistoryId { get; set; }
        public int EmpId { get; set; }
        public float BaseSalary { get; set; }
        public float? HousingAllowances { get; set; }
        public float? TransportationAllowances { get; set; }
        public float? OtherAllowances { get; set; }
        public float? Overtime { get; set; }
        public float? Bonus { get; set; }
        public float? DelayDiscount { get; set; }
        public float? AbsenceDiscount { get; set; }
        public float? OtherDiscount { get; set; }
        public float? Debt { get; set; }
        public float? SharedPortion { get; set; }
        public float? FacilityPortion { get; set; }
        public int WorkDays { get; set; }
        public DateTime Date { get; set; }
        public string ExchangeStatement { get; set; }
        public string? Notes { get; set; }
    }

}
