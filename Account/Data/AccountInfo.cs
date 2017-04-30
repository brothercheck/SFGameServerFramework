using SD.Infrastructure.EntityBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Data
{

    /// <summary>
    /// 账号注册信息
    /// </summary>
    public class AccountInfo: AggregateRootEntity
    {
        public AccountInfo()
        {

        }
        public AccountInfo(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 手机类型
        /// </summary>
        public byte MobileType { get; set; } = 0;
        /// <summary>
        /// 手机设备识别码  可以为空
        /// </summary>
        public string IMEI { get; set; } = string.Empty;

       
        public string RegisterIp { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDNumber { get; set; } = string.Empty;

        /// <summary>
        /// 身份证姓名
        /// </summary>
        public string IDName { get; set; } = string.Empty;
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// 渠道id
        /// </summary>
        public string RetailID { get; set; } = string.Empty;
    }

}
