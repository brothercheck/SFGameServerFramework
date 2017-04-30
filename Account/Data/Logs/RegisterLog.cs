using SD.Infrastructure.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Data.Logs
{
    public class RegisterLog : AggregateRootEntity
    {
        public RegisterLog() { }
        public RegisterLog(string pid, byte mobileType, string imei)
        {
            Name = pid;
            MobileType = mobileType;
            IMEI = imei;
        }

        public byte MobileType { get; set; }
        public string IMEI { get; set; }
    }
}
