using Account.Data.Logs;
using SD.Infrastructure.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.IRepositories
{
    interface ILoginLogRepository : ISimpleRepository<LoginLog>
    {
    }
}
