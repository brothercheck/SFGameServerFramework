using SD.Infrastructure.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Data.Logs
{
    public class LoginLog : AggregateRootEntity
    {
        public LoginLog() { }

        public LoginLog(string userId, string imei, string retialUser, string retialToken)
        {
            Name = userId;
            IMEI = imei;
            RetialUser = retialUser ?? string.Empty;
            RetialToken = retialToken ?? string.Empty;

        }
        public string IMEI { get; set; }
        public string RetialUser { get; set; }
        public string RetialToken { get; set; }
    }
}
