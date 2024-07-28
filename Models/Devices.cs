namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Devices
    {
        [Key]
        public int devices_id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
    }

}
