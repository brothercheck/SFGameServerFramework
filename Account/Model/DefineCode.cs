using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Model
{
    public static class DefineCode
    {
        /*
         0： OK，成功的
        100：服务器异常
        101：签名错误
        102：无处理程序
        103：密码错误
        105：登录凭证无效
        106: 登录凭证过期
        */
        public static string Version { get; set; } = "1.0.0";
        public const string Error = "账号服务器异常！";
        public const string IMEINullError = "IMEI为空！";
        public const string SignError = "签名错误！";
        public const string NoHandler = "无处理程序！";
        public const string PassworkLengthError = "密码长度错误！";
        public const string PassworkError = "密码错误！";
        public const string PasswordOrPassError = "账号或密码错误！";
        public const string RegistError = "账号已存在，注册失败！";
        public const string ChangePassError = "修改密码失败！";
        public const string NoToken = "登录凭证无效！";
        public const string TokenExpired = "登录凭证已过期！";
        public const string ParamsError = "请求参数错误";
    }

    public enum StateCode
    {
        //OK，成功的
        OK = 0,
        //服务器异常
        Error = 100,
        //签名错误
        SignError = 101,
        //无处理程序
        NoHandler = 102,
        //密码错误
        PassworkError = 103,
        //登录凭证无效
        NoToken = 105,
        //登录凭证过期
        TokenExpired = 106,
        //请求超时
        Timeout = 107,
        //转换json错误
        ParseError = 108,
        ParamsError = 109

    }
}
