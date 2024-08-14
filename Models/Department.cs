namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Department
    {
        [Key]
        public int departement_id { get; set; }
        public string departement_name { get; set; }

        [ForeignKey("employee_id")]
        public int supervisor_id { get; set; }

        [ForeignKey("supervisor_id")]
        public virtual employee Supervisor { get; set; }

    }
}