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

        string line = default(string);
        string amount = default(string);
        Regex re = new Regex(@"^(?<account>.*)\;(?<card>.*)\;(?<effectivedate>.*)\;(?<posteddate>.*)\;(?<description>.*)\;(?<reference>.*)\;(?<debit>.*)\;(?<credit>.*)$", RegexOptions.IgnoreCase);
        DateTime postedDate;
        DateTime effectiveDate;

        using (StreamReader sr = new StreamReader(_core.Filename))
        {
            // discard first line
            line = sr.ReadLine();
            if (line == null)
            {
                return;
            }

            while ((line = sr.ReadLine()) != null)
            {
                Match m = re.Match(line);
                if (m.Success)
                {
                    postedDate = DateTime.ParseExact(m.Groups["posteddate"].Value, @"MM/dd/yyyy", CultureInfo.InvariantCulture);
                    if (postedDate < _core.StartDate || postedDate > _core.EndDate)
                    {
                        continue;
                    }

                    _core.Debug(string.Format("{0} < {1} > {2}", _core.StartDate.ToString(), postedDate.ToString(), _core.EndDate.ToString()));

                    TransactionDTO txn = new TransactionDTO();
                    txn.Account = m.Groups["account"].Value;
                    txn.Account = txn.Account.Replace("-", "");

                    //txn.Date = DateTime.ParseExact(m.Groups["effectivedate"].Value, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    txn.Date = postedDate;

                    //txn.PostedDate = DateTime.ParseExact(m.Groups["posteddate"].Value, @"MM/dd/yyyy", CultureInfo.InvariantCulture);
                    txn.Description = m.Groups["description"].Value;
                    //txn.Reference = m.Groups["reference"].Value;
                    //txn.Reference = postedDate.ToString("MMM/dd/yyyy");
                    effectiveDate = DateTime.ParseExact(m.Groups["effectivedate"].Value, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    txn.Reference = effectiveDate.ToString("MMM/dd/yyyy");

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
