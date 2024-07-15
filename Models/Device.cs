namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

}
