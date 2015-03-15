using System;
using System.IO;
using System.Text;
using Retlang.Fibers;

namespace ofxware.Models
{
    public class Writer
    {
        #region Fields

        Bus _bus;
        ThreadFiber _fiber;

        #endregion

        #region Properties
        #endregion

        #region Constructor

        public Writer(Bus bus)
        {
            _bus = bus;

            _fiber = new ThreadFiber();
            _bus.OfxWriterChannel.Subscribe(_fiber, OnWrite);
        }

        #endregion

        #region Methods

        public void Start()
        {
            _fiber.Start();
        }

        public void Stop()
        {
            _fiber.Join();
        }

        private void OnWrite(Csv csv)
        {
            string template = ReadTemplate("ofxtemplate.txt");

            template = template.Replace("%DTSERVER%", DateTime.Now.ToString("yyyyMMddHHmmss"));
            template = template.Replace("%ORG%", csv.Account.Bank);
            template = template.Replace("%FID%", csv.Account.Bank);

            template = template.Replace("%CURDEF%", "USD");
            template = template.Replace("%BANKID%", csv.Account.Bank);

            template = template.Replace("%ACCTTYPE%", csv.Account.Type);

            template = template.Replace("%DTSTART%", csv.Account.StartDate.ToString("yyyyMMddHHmmss"));
            template = template.Replace("%DTEND%", csv.Account.EndDate.ToString("yyyyMMddHHmmss"));

            StringBuilder sb = new StringBuilder();
            string account = "";

            csv.Sort();

            foreach (Transaction txn in csv.Transactions)
            {
                account = txn.Account;

                sb.AppendLine("\t\t\t<STMTTRN>");
                sb.AppendLine("\t\t\t\t<TRNTYPE>OTHER");
                sb.AppendLine(string.Format("\t\t\t\t<DTPOSTED>{0}", txn.Date.ToString("yyyyMMdd120000")));
                sb.AppendLine(string.Format("\t\t\t\t<TRNAMT>{0}", txn.Credit - txn.Debit));
                sb.AppendLine(string.Format("\t\t\t\t<FITID>{0}", txn.Reference));
                sb.AppendLine(string.Format("\t\t\t\t<REFNUM>{0}", txn.Reference));
                sb.AppendLine(string.Format("\t\t\t\t<MEMO>{0}", txn.Description));
                sb.AppendLine("\t\t\t</STMTTRN>");

                if (csv.Account.Type == "CREDITLINE")
                {
                    csv.Account.Balance += txn.Debit - txn.Credit;
                }
                else
                {
                    csv.Account.Balance += txn.Credit - txn.Debit;
                }
            }

            csv.Account.Balance = Math.Round(csv.Account.Balance, 2);

            template = template.Replace("%ACCTID%", account);

            template = template.Replace("%TXNS%", sb.ToString());

            template = template.Replace("%BALAMT%", csv.Account.Balance.ToString());
            template = template.Replace("%DTASOF%", csv.Account.EndDate.AddDays(1).AddSeconds(-1).ToString("yyyyMMddHHmmss"));

            csv.Account.Dirty = true;

            using (StreamWriter sw = new StreamWriter(csv.Filename + ".ofx"))
            {
                sw.Write(template);
            };

            _bus.AccountFinishedChannel.Publish(0);
        }

        private string ReadTemplate(string filepath)
        {
            string template = null;

            using (StreamReader sr = new StreamReader(filepath))
            {
                template = sr.ReadToEnd();
            }

            return template;
        }

        #endregion

    }
}
