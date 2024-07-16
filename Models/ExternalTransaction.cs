namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ExternalTransaction
    {
        [Key]
        public int external_transactions_id { get; set; }
        public string name { get; set; }
        public int identity_number { get; set; }
        public string status { get; set; }
        public string communication { get; set; }
        public string case_status { get; set; }
        public string sending_party { get; set; }
        public DateTime receiving_date { get; set; }
        public DateTime sending_date { get; set; }
        public int sending_number { get; set; }
        public int receiving_number { get; set; }
    }

}
