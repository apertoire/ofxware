using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Retlang.Fibers;

namespace ofxware.Models
{
    public class Engine
    {
        #region Fields

        Bus _bus;
        ThreadFiber _fiber;

        NotificationCollection<Account> _accounts;

        #endregion

        #region Properties

        public NotificationCollection<Account> Accounts
        {
            get { return _accounts; }
        }

        #endregion

        #region Constructor

        public Engine(Bus bus)
        {
            _bus = bus;

            _fiber = new ThreadFiber();
            _bus.CsvProcessorChannel.Subscribe(_fiber, OnProcess);
            _bus.AccountFinishedChannel.Subscribe(_fiber, OnFinished);

            _accounts = new NotificationCollection<Account>();
        }

        #endregion

        #region Methods

        public void Start()
        {
            _fiber.Start();

            Load();
        }

        private void Load()
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                settings.XmlResolver = null;
                settings.ValidationType = ValidationType.None;
                settings.ProhibitDtd = false;

                Account account = null;

                using (XmlReader reader = XmlReader.Create("ofxware.accounts", settings))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "account" && reader.NodeType == XmlNodeType.Element)
                        {
                            account = new Account(reader["bank"], reader["number"], reader["type"]);
                        }

                        if (reader.Name == "startdate" && reader.NodeType == XmlNodeType.Element)
                        {
                            reader.Read();
                            account.PrevStartDate = DateTime.ParseExact(reader.Value, "MMM dd, yyyy", CultureInfo.InvariantCulture);
                        }

                        if (reader.Name == "enddate" && reader.NodeType == XmlNodeType.Element)
                        {
                            reader.Read();
                            account.PrevEndDate = DateTime.ParseExact(reader.Value, "MMM dd, yyyy", CultureInfo.InvariantCulture);
                            account.StartDate = account.PrevEndDate.AddDays(1);
                            account.EndDate = account.StartDate.AddMonths(1).AddDays(-(account.StartDate.Day));
                        }

                        if (reader.Name == "balance" && reader.NodeType == XmlNodeType.Element)
                        {
                            reader.Read();
                            account.PrevBalance = double.Parse(reader.Value, NumberStyles.Currency);
                            account.Balance = account.PrevBalance;
                        }

                        if (reader.Name == "account" && reader.NodeType == XmlNodeType.EndElement)
                        {
                            _accounts.Add(account);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            _fiber.Join();
        }

        private void OnProcess(AccountDTO dto)
        {
            Csv csv = new Csv();

            csv.Account = dto.Account;
            csv.Filename = dto.Filename;

            var reply = _bus.CsvParserChannel.SendRequest(csv);
            reply.Receive(100000, out csv);

            if (csv == null)
            {
                return;
            }

            _bus.OfxWriterChannel.Publish(csv);
        }

        private void OnFinished(int state)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            try
            {
                using (XmlWriter writer = XmlWriter.Create("ofxware.accounts.tmp", settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("accounts");

                    foreach (Account account in _accounts)
                    {
                        writer.WriteStartElement("account");
                        writer.WriteAttributeString("bank", account.Bank);
                        writer.WriteAttributeString("number", account.Number);
                        writer.WriteAttributeString("type", account.Type);

                        writer.WriteStartElement("startdate");
                        if (account.Dirty)
                        {
                            writer.WriteString(account.StartDate.ToString("MMM dd, yyyy"));
                        }
                        else
                        {
                            writer.WriteString(account.PrevStartDate.ToString("MMM dd, yyyy"));
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("enddate");
                        if (account.Dirty)
                        {
                            writer.WriteString(account.EndDate.ToString("MMM dd, yyyy"));
                        }
                        else
                        {
                            writer.WriteString(account.PrevEndDate.ToString("MMM dd, yyyy"));
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("balance");
                        writer.WriteString(account.Balance.ToString());
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.Flush();
                }

                if (File.Exists("ofxware.accounts"))
                {
                    MoveFile("ofxware.accounts", "ofxware.accounts.bak");
                }

                MoveFile("ofxware.accounts.tmp", "ofxware.accounts");
            }
            catch (Exception)
            {
                //Logging.Instance.LogError("MNGR", string.Format("Error saving session file: {0} ({1})", e.Message, e.StackTrace));
            }
        }

        public void MoveFile(string src, string dst)
        {
            if (File.Exists(dst))
            {
                File.Delete(dst);
            }

            File.Copy(src, dst);
            File.Delete(src);
            //File.Move(src, dst);
        }


        #endregion
    }
}
