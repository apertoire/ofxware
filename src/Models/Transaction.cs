using System;

namespace ofxware.Models
{
    public class Transaction
    {
        #region Properties

        public string Account { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }

        #endregion

        #region Constructor

        public Transaction(string account, DateTime date, string description, string reference, double credit, double debit)
        {
            Account = account;
            Date = date;
            Description = description;
            Reference = reference;
            Credit = credit;
            Debit = debit;
        }

        #endregion
    }
}
