namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ExternalTransaction
    {
        [Key]
        public int ExternalTransactionId { get; set; }
        public string Name { get; set; }
        public int IdentityNumber { get; set; }
        public string Status { get; set; }
        public string Communication { get; set; }
        public string CaseStatus { get; set; }
        public string SendingParty { get; set; }
        public DateTime ReceivingDate { get; set; }
        public DateTime SendingDate { get; set; }
        public int SendingNumber { get; set; }
        public int ReceivingNumber { get; set; }
    }

}
