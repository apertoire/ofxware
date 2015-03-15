using System;
using System.ComponentModel;
using Retlang.Fibers;
using Openware.Helpers;

namespace ofxware.Models
{
    public class Core : Bindable
    {
        #region Fields

        Bus _bus;
        ThreadFiber _fiber;

        Engine _engine;
        Parser _parser;
        Writer _writer;

        #endregion

        #region Properties

        public NotificationCollection<Account> Accounts
        {
            get { return _engine.Accounts; }
        }

        #endregion

        #region Constructor

        public Core()
        {
            _bus = new Bus();

            _engine = new Engine(_bus);
            _parser = new Parser(_bus);
            _writer = new Writer(_bus);
            
            _fiber = new ThreadFiber();
            _bus.StartCoreChannel.Subscribe(_fiber, OnStart);
            _bus.StopCoreChannel.Subscribe(_fiber, OnStop);
        }

        #endregion

        #region Methods

        public void Start()
        {
            _fiber.Start();
            _bus.StartCoreChannel.Publish(0);
        }

        public void Process(string filename, Account account)
        {
            _bus.CsvProcessorChannel.Publish(new AccountDTO(filename, account));
        }

        private void OnStart(int state)
        {
            _writer.Start();
            _parser.Start();
            _engine.Start();
        }

        private void OnStop(int state)
        {
            _engine.Stop();
            _parser.Stop();
            _writer.Stop();
        }

        #endregion
    }
}
