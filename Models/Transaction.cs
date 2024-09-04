namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Transaction
    {
        [Key]
        public int transaction_id { get; set; }
        public DateTime? create_date { get; set; }
        public DateTime? close_date { get; set; }
        public string? status { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public string? files { get; set; }
        public int from_emp_id { get; set; }
        public int? to_emp_id { get; set; }
        public int department_id { get; set; }
        [ForeignKey("department_id")]
        public virtual Department Department { get; set; }
        [ForeignKey("from_emp_id")]
        public virtual employee_details Employee_detail { get; set; }

        public string? Confidentiality { get; set; }
        public string? Urgency { get; set; }
        public string? Importance { get; set; }
      

        // New property for referrals
        //public virtual ICollection<Referral> Referrals { get; set; }
        public ICollection<Referral> Referrals { get; set; }

    }

}
