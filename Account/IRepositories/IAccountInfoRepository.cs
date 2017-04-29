using Account.Data;
using SD.Infrastructure.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.IRepositories
{

    /// <summary>
    /// 账号信息仓储接口
    /// </summary>
    public interface IAccountInfoRepository:ISimpleRepository<AccountInfo>
    {
    }
}
