using System;
using Openware.Helpers;

namespace ofxware.Models
{
    public class Account : Bindable
    {
        #region Properties

        public string Bank { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public DateTime PrevStartDate { get; set; }
        public DateTime PrevEndDate{ get; set; }
        public double PrevBalance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Balance { get; set; }
        public bool Dirty { get; set; }

        #endregion

        #region Constructor

        public Account(string bank, string number, string type)
        {
            Bank = bank;
            Number = number;
            Type = type;
            Dirty = false;
        }

        #endregion
    }
}
