using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharityProject.Models
{
    public class Referral
    {
        [Key]
        public int referral_id { get; set; }

        [Required]
        public int transaction_id { get; set; }

        [ForeignKey("transaction_id")]
        public Transaction Transaction { get; set; }

        [Required]
        public int from_employee_id { get; set; }

        [ForeignKey("from_employee_id")]
        public employee from_employee { get; set; }

        [Required]
        public int to_employee_id { get; set; }

        [ForeignKey("to_employee_id")]
        public employee to_employee { get; set; }

        [Required]
        public DateTime referral_date { get; set; }

        public string? comments { get; set; }
    }
}