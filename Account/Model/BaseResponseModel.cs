using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Model
{
    /// <summary>
    /// 基本返回值
    /// </summary>
    public class BaseResponseModel
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int StateCode { get; set; } = 0;

        /// <summary>
        /// 状态详细描述
        /// </summary>
        public string StateDescription { get; set; } = string.Empty;

        /// <summary>
        /// 版本信息
        /// </summary>
        public string Vesion { get; set; } = "1.0";

        /// <summary>
        /// 处理程序
        /// </summary>
        public string Handler { get; set; } = string.Empty;

        public Object Data { get; set; }
    }
}
