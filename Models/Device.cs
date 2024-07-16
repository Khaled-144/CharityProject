namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Device
    {
        [Key]
        public int devices_id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
    }

}
