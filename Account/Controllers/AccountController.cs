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

namespace Account.Controllers
{
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }


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
        public async Task<BaseResponseModel> Register(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string sign, int mobileType = -1, bool isCustom = true)
        {
            #region 验证参数
            if ((string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(pwd)) || mobileType < 0)
            {
                return new BaseResponseModel()
                {
                    Handler = "Register",
                    StateCode = (int)StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region 验证Sign
            string paramsUrl = "MobileType=" + mobileType.ToString() + "&Pid=" + pid + "&Pwd=" + pwd + "&IMEI="
                + imei + "&ScreenX=" + screenX.ToString() + "&ScreenY=" + screenY.ToString() + "&RetailID=" + retailId;
            if (!MD5Helper.VerifyMd5Hash(paramsUrl + Config.SignKey, sign))
            {
                return new BaseResponseModel()
                {
                    Handler = "Register",
                    StateCode = (int)StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion
            //TODO:
            Guid token = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Task tokenTask = Task.Run(() => InsertTokenList(token, userId));  //生成token 并写入Token列表
            Task regTask = Task.Run(() => Register(pid, pwd));                  //注册账号
            Task logTask = Task.Run(() => InsertRegLog(pid, pwd, imei, mobileType, userId));//写入日志

            return new BaseResponseModel()
            {
                Handler = "Register",
                Data = new { Token = token, UserId = userId }
            };
        }

        private void InsertRegLog(string pid, string pwd, string imei, int mobileType, Guid userId)
        {
            //TODO
            //异常处理
        }


        /// <summary>
        /// 写入Token数据到表中
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userId">userid</param>
        private void InsertTokenList(Guid token, Guid userId)
        {
            //TODO
            //异常处理
        }


        /// <summary>
        /// 注册新用户账号
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private void Register(string pid, string pwd)
        {
            //TODO
            //异常处理
        }

        [HttpGet]

        public BaseResponseModel Login()
        {


            return new BaseResponseModel()
            {
                Handler = "Register",
                Data = new { Token = "3074e807e73c4a899facb455aab1725e", UserId = Guid.NewGuid() }
            };
        }
    }
}