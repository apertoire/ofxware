using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Openware.Plugins;


public class BGLoader : IPlugin
{
    ICore _core = null;

    public void Run(ICore core)
    {
        _core = core;

        string line;
        string amount;
        string account;
        Regex re = new Regex(@"^(?<date>.*)\|(?<description>.*)\|(?<reference>.*)\|(?<debit>.*)\|(?<credit>.*)\|(?<balance>.*)$", RegexOptions.IgnoreCase);
        DateTime date;

        using (StreamReader sr = new StreamReader(_core.Filename))
        {
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("Fecha"))
                {
                    break;
                }
                else if (line.StartsWith("Cuenta:"))
                {
                    account = line.Substring(8, 15);
                }
            }

            while ((line = sr.ReadLine()) != null)
            {
                Match m = re.Match(line);
                if (m.Success)
                {
                    date = DateTime.ParseExact(m.Groups["date"].Value, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (date < _core.StartDate || date > _core.EndDate)
                    {
                        continue;
                    }

                    //_core.Debug(string.Format("{0} < {1} > {2}", _core.StartDate.ToString(), postedDate.ToString(), _core.EndDate.ToString()));

                    TransactionDTO txn = new TransactionDTO();
                    //txn.Account = m.Groups["account"].Value;
                    //txn.Account = txn.Account.Replace("-", "");
                    txn.Account = account;

                    txn.Date = date;
                    txn.Description = m.Groups["description"].Value;
                    txn.Reference = m.Groups["reference"].Value;

                    amount = m.Groups["debit"].Value;
                    if (string.IsNullOrEmpty(amount))
                    {
                        txn.Debit = 0.0;
                    }
                    else
                    {
                        txn.Debit = double.Parse(amount, NumberStyles.Currency);
                    }

                    amount = m.Groups["credit"].Value;
                    if (string.IsNullOrEmpty(amount))
                    {
                        txn.Credit = 0.0;
                    }
                    else
                    {
                        txn.Credit = double.Parse(amount, NumberStyles.Currency);
                    }

                    _core.AddTransaction(txn);
                }
            }
        };

    }
}
