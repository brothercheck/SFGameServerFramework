using Account.Data;
using Account.IRepositories;
using SD.Infrastructure.Repository.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Repositories
{
    /// <summary>
    /// 账号信息仓储实现
    /// </summary>
    public class AccountInfoRepository: MongoRepositoryProvider<AccountInfo>, IAccountInfoRepository
    {
    }
}
