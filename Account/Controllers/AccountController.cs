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
        /// ע���¼�
        /// </summary>
        /// <param name="pid">�˺�</param>
        /// <param name="pwd">����</param>
        /// <param name="imei">�ֻ��豸��</param>
        /// <param name="screenX">��Ļ���</param>
        /// <param name="screenY">��Ļ�߶�</param>
        /// <param name="retailId">����id</param>
        /// <param name="sign">signǩ���ַ���</param>
        /// <param name="mobileType">�ֻ�����</param>
        /// <param name="isCustom">�Ƿ�������ע��</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponseModel> Register(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string sign, int mobileType = -1, bool isCustom = true)
        {
            #region ��֤����
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

            #region ��֤Sign
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
            Task tokenTask = Task.Run(() => InsertTokenList(token, userId));  //����token ��д��Token�б�
            Task regTask = Task.Run(() => Register(pid, pwd));                  //ע���˺�
            Task logTask = Task.Run(() => InsertRegLog(pid, pwd, imei, mobileType, userId));//д����־

            return new BaseResponseModel()
            {
                Handler = "Register",
                Data = new { Token = token, UserId = userId }
            };
        }

        private void InsertRegLog(string pid, string pwd, string imei, int mobileType, Guid userId)
        {
            //TODO
            //�쳣����
        }


        /// <summary>
        /// д��Token���ݵ�����
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userId">userid</param>
        private void InsertTokenList(Guid token, Guid userId)
        {
            //TODO
            //�쳣����
        }


        /// <summary>
        /// ע�����û��˺�
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private void Register(string pid, string pwd)
        {
            //TODO
            //�쳣����
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