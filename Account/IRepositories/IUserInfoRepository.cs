using Account.Data;
using SD.Infrastructure.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.IRepositories
{
    public interface IUserInfoRepository : ISimpleRepository<UserInfo>
    {
    }
}
