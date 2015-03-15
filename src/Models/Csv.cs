using System;
using System.Collections.Generic;

namespace ofxware.Models
{
    public class Csv
    {
        #region Fields

        List<Transaction> _transactions;

        #endregion

        #region Properties

        public List<Transaction> Transactions
        {
            get { return _transactions; }
        }

        public Account Account { get; set; }
        public string Filename { get; set; }

        #endregion

        #region Constructor

        public Csv()
        {
            _transactions = new List<Transaction>();
        }

        #endregion

        #region Methods

        public void AddTransaction(Transaction txn)
        {
            _transactions.Add(txn);
        }

        public void Sort()
        {
            _transactions.Sort(CompareByDate);
        }

        private static int CompareByDate(Transaction x, Transaction y)
        {
            int ret = 0;

            if (x.Date < y.Date)
            {
                ret = -1;
            }
            else
            {
                if (x.Date > y.Date)
                {
                    ret = 1;
                }
            }

            return ret;
        }

        #endregion
    }
}
