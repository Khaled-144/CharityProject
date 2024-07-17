using System.ComponentModel.DataAnnotations;

namespace CharityProject.Models
{
    public class OtherService
    {
        [Key]
        public int service_id { get; set; }
        public string service_name { get; set; }
    }

}
