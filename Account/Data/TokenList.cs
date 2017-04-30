﻿using SD.Infrastructure.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Data
{
    public class TokenList : AggregateRootEntity
    {
        public TokenList()
        {

        }
        public TokenList(string pid, Guid token)
        {
            Name = pid;
            Token = token;
            SavedTime = DateTime.Now;
        }
        public Guid Token { get; set; }

        //更新token
        public void Update(Guid token)
        {
            Token = token;
            SavedTime = DateTime.Now;
        }
    }
}
