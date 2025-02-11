namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class charter
    {
        [Key]
        public int charter_id { get; set; }
        public string charter_info { get; set; }  
      
        public double? serial_number { get; set; }
       
        public DateOnly creation_date { get; set; } 
        public string from_departement_name { get; set; }  
        public string status { get; set; }

        public string notes { get; set; }  
        public string to_departement_name { get; set; }

        [ForeignKey("to_emp_id")]
        public virtual employee employee { get; set; }
        public int to_emp_id { get; set; }
      
        public DateTime? receive_date { get; set; }  
        public DateTime? end_date { get; set; }  






       
       
    }
}

