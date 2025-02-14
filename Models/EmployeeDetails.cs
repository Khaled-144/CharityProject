﻿namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmployeeDetails
    {
        [Key]
        public int employee_details_id { get; set; }
        public int identity_number { get; set; }
        public int departement_id { get; set; }
        public string position { get; set; }
        public string permission_position { get; set; }
        public string contract_type { get; set; }
        public string? national_address { get; set; }
        public string? education_level { get; set; }
        public DateTime hire_date { get; set; }
        public DateTime? leave_date { get; set; }
        public bool active { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string gender { get; set; }
        public string? files { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee employee_id { get; set; }
    }

}
