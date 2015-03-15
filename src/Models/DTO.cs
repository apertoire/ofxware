using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ofxware.Models
{
    public class AccountDTO
    {
        public string Filename { get; private set; }
        public Account Account { get; private set; }

        public AccountDTO(string filename, Account account)
        {
            Filename = filename;
            Account = account;
        }
    }
}
