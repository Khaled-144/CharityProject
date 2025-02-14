﻿namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Transaction
    {
        [Key]
        public int transaction_id { get; set; }
        public string type { get; set; }
        public DateTime create_date { get; set; }
        public DateTime? close_date { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public string? files { get; set; }
        public int from_emp_id { get; set; }
        public int? to_emp_id { get; set; }
        public int department_id { get; set; }
    }

}
