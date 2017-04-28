using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Data
{

    /// <summary>
    /// 注册参数请求
    /// </summary>
    public class RegisterReq
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 手机类型
        /// </summary>
        public byte MobileType { get; set; }
        /// <summary>
        /// 手机设备识别码  可以为空
        /// </summary>
        public string IMEI { get; set; } = string.Empty;

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public int ScreenX { get; set; }
        /// <summary>
        /// 屏幕高度
        /// </summary>
        public int ScreenY { get; set; }

        /// <summary>
        /// 渠道id
        /// </summary>
        public string RetailID { get; set; }

        /// <summary>
        /// 自定义账号
        /// </summary>
        public bool IsCustom { get; set; } = true;
    }

    /// <summary>
    /// 注册参数返回值
    /// </summary>
    public class RegisterRep
    {
        public string  Token { get; set; }
        public Guid UserId { get; set; }
    }
}
