using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Account.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            string string1 = "hello";

            return new string[] { $"{string1}", "value2" };
        }
    }
}