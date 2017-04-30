using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Account.Model;
using System.Text;
using System.Security.Cryptography;
using Common;
using Account.Data;
using Account.IRepositories;
using Account.Repositories;
using System.Diagnostics;
using Account.Data.Logs;

namespace Account.Controllers
{
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IAccountInfoRepository _accountInfoRep;
        private readonly IUserInfoRepository _userInfoRep;
        private readonly ITokenListRepository _tokenListRep;
        private readonly IRegisterLogRepository _regLogRep;
        public AccountController()
        {
            _accountInfoRep = new AccountInfoRepository();
            _userInfoRep = new UserInfoRepository();
            _tokenListRep = new TokenListRepository();
            _regLogRep = new RegisterLogRepository();
        }


        #region 账号注册  


        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="pid">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="imei">手机设备码</param>
        /// <param name="screenX">屏幕宽度</param>
        /// <param name="screenY">屏幕高度</param>
        /// <param name="retailId">渠道id</param>
        /// <param name="sign">sign签名字符串</param>
        /// <param name="mobileType">手机类型</param>
        /// <param name="isCustom">是否是自主注册</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponseModel> Register(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string sign, byte mobileType = 0, bool isCustom = true)
        {
            #region 非空验证
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(pwd))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Register),
                    StateCode = StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region 验证Sign
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Register),
                    StateCode = StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region 执行
            //TODO:
            Guid token = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            BaseResponseModel result = new BaseResponseModel();
            bool tag = false;
            try
            {
                Task tokenTask = Task.Run(() => InsertTokenList(token, pid));  //生成token 并写入Token列表
                var regTask = Register(pid, pwd, mobileType, imei, HttpContext.Connection.RemoteIpAddress.ToString());       //注册账号
                Task logTask = Task.Run(() => InsertRegLog(pid, imei, mobileType));//写入日志
                switch (regTask)
                {
                    case -1:
                        tag = true;
                        result = new BaseResponseModel()
                        {
                            StateCode = StateCode.RegistError,
                            Vesion = DefineCode.Version,
                            StateDescription = DefineCode.RegistError,
                            Handler = nameof(AccountController.Register),
                            Data = null
                        }; break;
                    case -2:
                        tag = true;
                        result = new BaseResponseModel()
                        {
                            StateCode = StateCode.RegistError,
                            Vesion = DefineCode.Version,
                            StateDescription = DefineCode.ParamsInvaild,
                            Handler = nameof(AccountController.Register),
                            Data = null
                        }; break;
                    default: break;
                }
            }

            catch (Exception ex)
            {
                tag = true;
                result = new BaseResponseModel()
                {
                    StateCode = StateCode.RegistError,
                    StateDescription = ex.Message,
                    Handler = nameof(AccountController.Register),
                    Data = null
                };
            }
            #endregion
            if (!tag)
                result = new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Register),
                    Data = new { Token = token, UserId = userId }
                };
            return result;
        }

        #region 检查签名 CheckSign(HttpContext httpContext, string sign)
        /// <summary>
        /// 检查签名
        /// </summary>
        /// <param name="httpContext">当前连接对象</param>
        /// <param name="sign">签名字串</param>
        /// <returns></returns>
        private bool CheckSign(HttpContext httpContext, string sign)
        {
            string paramsUrl = httpContext.Request.QueryString.ToUriComponent();
            var index = paramsUrl.IndexOf("&sign");
            string paramNonSign = paramsUrl.Substring(1, index - 1);
            if (MD5Helper.VerifyMd5Hash(paramNonSign + Config.SignKey, sign))
                return true;
            return false;
        }
        #endregion


        /// <summary>
        /// 写入注册日志
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="imei"></param>
        /// <param name="mobileType"></param>
        private void InsertRegLog(string pid, string imei, byte mobileType)
        {
            _regLogRep.Add(new RegisterLog(pid, mobileType, imei));

            //TODO
            //异常处理
        }

        /// <summary>
        /// 写入Token数据到表中
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userId">userid</param>
        private void InsertTokenList(Guid token, string userId)
        {
            //检查userid 是否已存在
            bool isExists = _tokenListRep.ExistsName(userId);
            //存在 更新token
            if (isExists)
            {
                var isExistsToken = _tokenListRep.SingleByName(userId);
                isExistsToken.Update(token);
                _tokenListRep.Save(isExistsToken);
            }
            else
            {
                //不存在，新增 userid 和token
                TokenList tokenList = new TokenList(userId, token);
                _tokenListRep.Add(tokenList);
            }
            //TODO
            //异常处理
        }


        /// <summary>
        /// 注册新用户账号
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private int Register(string pid, string pwd, byte mobileType, string imei, string registerIp)
        {
            //检查账号是否重复
            if (_accountInfoRep.ExistsName(pid))
            {
                return -1;
            }
            bool arg = false;
            //写入角色表  创建新角色
            AccountInfo account = new AccountInfo(pid)
            {
                Password = pwd,
                IMEI = imei,
                RegisterIp = registerIp
            };
            try
            { //插入数据库
                _accountInfoRep.Add(account);
            }
            catch (ArgumentNullException ex)
            {
                arg = true;
            }
            if (arg)
            {
                return -2;
            }
            return 0;
        }
        #endregion


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="pid">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="imei">设备码</param>
        /// <param name="screenX">宽度</param>
        /// <param name="screenY">高度</param>
        /// <param name="retailId">第三方id</param>
        /// <param name="retialUser">第三方账号</param>
        /// <param name="retianToken">第三方token</param>
        /// <param name="sign">签名字串</param>
        /// <param name="mobileType">手机类型</param>
        /// <returns></returns>
        [HttpGet]

        public BaseResponseModel Login(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string retialUser, string retianToken, string sign, int mobileType = -1)
        {

            /*
                请求参数
                MobileType：【必传】Int整型，手机类型
                Pid：【必传】String类型，获取账号接口下发的账号，第三方登录可以为空
                Pwd：【必传】String类型，获取账号接口下发的密码，第三方登录可以为空
                IMEI：String类型，手机设备识别码，可以为空；不为空时如果上次有注册过，返回上次使用的账号
                ScreenX：Int整型，手机屏幕宽度
                ScreenY：Int整型，手机屏幕高度
                RetailID：String类型，客户打包的渠道编号，服务端与客户端协商确定
                IsCustom：Bool类型，账号使用自定义的，True为启用；
                RetialUser:String类型，第三方登录的账号ID
                RetialToken:String类型，第三方登录的账号身份凭证
                响应参数
                Token：String类型，登录授权成功获得的凭证，有效期（24H）
                UserID：GUID，用户的唯一编号
                示例
                http://127.0.0.1:80/Login?MobileType=0&Pid=Z17465&Pwd=564613&IMEI=00:02:32:65&ScreenX=&ScreenY=&RetailID=&sign=c792a4eb8a7761524ea6e512f0efc939
             */

            #region 非空验证
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(pwd) || mobileType < 0)
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Login),
                    StateCode = StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region 验证Sign
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Login),
                    StateCode = StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region 验证账号密码
            switch (CheckId(pid, pwd))
            {
                case -1:
                    return new BaseResponseModel()
                    {
                        Handler = nameof(AccountController.Login),
                        StateCode = StateCode.PasswordError,
                        StateDescription = DefineCode.PasswordError,
                        Vesion = DefineCode.Version,
                        Data = null
                    };
                case -2: break;
            }

            #endregion

            #region 执行DB操作
            Guid token = Guid.NewGuid();
            Task tokenTask = Task.Run(() => InsertTokenList(token, pid)); //写入token列表
            Task logTask = Task.Run(() => InsertLoginLog(pid, imei, retialUser, retianToken));
            #endregion

            return new BaseResponseModel()
            {
                Handler = nameof(AccountController.Login),
                Data = new { Token = token }
            };
        }


        /// <summary>
        /// 检查账号密码是否正确
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private int CheckId(string pid, string pwd)
        {
            //查找 pid
            if (_accountInfoRep.ExistsName(pid))
            {//账号存在,验证密码
                if (!true)
                {
                    return -1;
                }
                return 0;
            }
            else
            {   //d账号不存在
                return -2;
            }
        }

        /// <summary>
        /// 写入登陆日志
        /// </summary>
        /// <param name="userId">账号</param>
        /// <param name="imei">设备码</param>
        /// <param name="retialUser">第三方账号</param>
        /// <param name="retianToken">第三方token</param>
        private void InsertLoginLog(string userId, string imei, string retialUser, string retianToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 一键注册 自动注册接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponseModel Passport()
        {

            string token = Guid.NewGuid().ToString();
            return new BaseResponseModel()
            {
                Handler = nameof(AccountController.Passport),
                Data = new { Token = token }
            };
        }
    }
}