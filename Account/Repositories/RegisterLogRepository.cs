using Account.Data.Logs;
using Account.IRepositories;
using SD.Infrastructure.Repository.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Repositories
{
    public class RegisterLogRepository : MongoRepositoryProvider<RegisterLog>, IRegisterLogRepository
    {
    }
}
