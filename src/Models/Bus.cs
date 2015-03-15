using System;
using Retlang.Channels;

namespace ofxware.Models
{
    public class Bus
    {
        #region Fields

        private readonly Channel<int> _startCoreChannel;
        private readonly Channel<int> _stopCoreChannel;

        private readonly Channel<AccountDTO> _csvProcessorChannel;
        private readonly RequestReplyChannel<Csv, Csv> _csvParserChannel;
        private readonly Channel<Csv> _ofxWriterChannel;
        private readonly Channel<int> _accountFinishedChannel;

        #endregion

        #region Properties

        public Channel<int> StartCoreChannel
        {
            get { return _startCoreChannel; }
        }

        public Channel<int> StopCoreChannel
        {
            get { return _stopCoreChannel; }
        }

        public Channel<AccountDTO> CsvProcessorChannel
        {
            get { return _csvProcessorChannel; }
        }

        public RequestReplyChannel<Csv, Csv> CsvParserChannel
        {
            get { return _csvParserChannel; }
        }

        public Channel<Csv> OfxWriterChannel
        {
            get { return _ofxWriterChannel; }
        }

        public Channel<int> AccountFinishedChannel
        {
            get { return _accountFinishedChannel; }
        }

        #endregion

        #region Constructor

        public Bus()
        {
            _startCoreChannel = new Channel<int>();
            _stopCoreChannel = new Channel<int>();

            _csvProcessorChannel = new Channel<AccountDTO>();
            _csvParserChannel = new RequestReplyChannel<Csv, Csv>();
            _ofxWriterChannel = new Channel<Csv>();

            _accountFinishedChannel = new Channel<int>();
        }

        #endregion

        #region Methods
        #endregion
    }
}
