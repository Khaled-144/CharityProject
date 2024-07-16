namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Department
    {
        [Key]
        public int departement_id { get; set; }
        public string departement_name { get; set; }
        public int supervisor_id { get; set; }
    }

}
