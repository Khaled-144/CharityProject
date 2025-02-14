﻿namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Charter
    {
        [Key]
        public int charter_id { get; set; }
        public int to_emp_id { get; set; }
        public string devices { get; set; }
        public DateTime start_date { get; set; }
        public string status { get; set; }
        public DateTime? end_date { get; set; }
    }

}
