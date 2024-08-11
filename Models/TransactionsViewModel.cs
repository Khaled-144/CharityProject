namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TransactionsViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public int InternalCount { get; set; }
        public int HolidaysCount { get; set; }
        public int LettersCount { get; set; }
        public int AssetsCount { get; set; }
    }

}
