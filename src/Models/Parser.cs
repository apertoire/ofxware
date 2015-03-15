using System;
using Retlang.Channels;
using Retlang.Fibers;
using Openware.Plugins;
using Openware.Helpers;

namespace ofxware.Models
{
    class Parser : ICore
    {
        #region Fields

        Bus _bus;
        ThreadFiber _fiber;

        Csv _csv;

        #endregion

        #region Properties
        #endregion

        #region Constructor

        public Parser(Bus bus)
        {
            _bus = bus;

            _fiber = new ThreadFiber();
            _bus.CsvParserChannel.Subscribe(_fiber, OnParse);
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

        private void OnParse(IRequest<Csv, Csv> request)
        {
            _csv = request.Request;

            PluginManager pm = new PluginManager(this);
            pm.RunPlugin(string.Format("{0}.cs", _csv.Account.Bank));

            //_csv.Sort();

            request.SendReply(_csv);
        }

        #endregion

        #region ICore implementation

        public string Filename
        {
            get { return _csv.Filename; }
        }

        public DateTime StartDate
        {
            get { return _csv.Account.StartDate; }
        }

        public DateTime EndDate
        {
            get { return _csv.Account.EndDate; }
        }

        public void AddTransaction(TransactionDTO txn)
        {
            _csv.AddTransaction(new Transaction(txn.Account, txn.Date, txn.Description, txn.Reference, txn.Credit, txn.Debit));
        }

        public void Debug(string text)
        {
            string some = string.Format("{0} is the new wave", text);
            Console.WriteLine(some);
        }

        #endregion

    }
}
