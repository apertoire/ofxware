using System;
using System.Collections.Generic;

namespace Openware.Plugins
{
    public interface ICore
    {
        string Filename { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }

        void AddTransaction(TransactionDTO transaction);
        void Debug(string text);
    }

    public interface IPlugin
    {
        void Run(ICore core);
    }

    public class TransactionDTO
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

        public TransactionDTO()
        {
        }

        #endregion
    }
}
