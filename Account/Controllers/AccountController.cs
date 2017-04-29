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

namespace Account.Controllers
{
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private IAccountInfoRepository _accountInfoRep;
        public AccountController()
        {
            _accountInfoRep = new AccountInfoRepository();
        }


        #region �˺�ע��  


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
        public async Task<BaseResponseModel> Register(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string sign, byte mobileType = 0, bool isCustom = true)
        {
            #region �ǿ���֤
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(pwd))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Register),
                    StateCode = (int)StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region ��֤Sign
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Register),
                    StateCode = (int)StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region ִ��
            //TODO:
            Guid token = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Task tokenTask = Task.Run(() => InsertTokenList(token, pid));  //����token ��д��Token�б�
            Task regTask = Task.Run(() => Register(pid, pwd, mobileType, imei, HttpContext.Connection.RemoteIpAddress.ToString()));       //ע���˺�
            Task logTask = Task.Run(() => InsertRegLog(pid, pwd, imei, mobileType, userId));//д����־
            #endregion

            #region ����
            return new BaseResponseModel()
            {
                Handler = nameof(AccountController.Register),
                Data = new { Token = token, UserId = userId }
            };
            #endregion

        }

        #region ���ǩ�� CheckSign(HttpContext httpContext, string sign)
        /// <summary>
        /// ���ǩ��
        /// </summary>
        /// <param name="httpContext">��ǰ���Ӷ���</param>
        /// <param name="sign">ǩ���ִ�</param>
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
        private void InsertTokenList(Guid token, string userId)
        {
            //���userid �Ƿ��Ѵ���

            //TODO
            DateTime dateTime = DateTime.Now;
            //�쳣����
        }


        /// <summary>
        /// ע�����û��˺�
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private void Register(string pid, string pwd, byte mobileType, string imei, string registerIp)
        {
            //TODO
            //д���ɫ��  �����½�ɫ
            AccountInfo account = new AccountInfo()
            {
                UserId = pid,
                Password = pwd,
                IMEI = imei,
                RegisterIp = registerIp
            };

            try
            { //�������ݿ�
                _accountInfoRep.Add(account);
            }
            catch (Exception ex)
            {
                //�쳣����

            }
        }
        #endregion


        /// <summary>
        /// �û���¼
        /// </summary>
        /// <param name="pid">�˺�</param>
        /// <param name="pwd">����</param>
        /// <param name="imei">�豸��</param>
        /// <param name="screenX">���</param>
        /// <param name="screenY">�߶�</param>
        /// <param name="retailId">������id</param>
        /// <param name="retialUser">�������˺�</param>
        /// <param name="retianToken">������token</param>
        /// <param name="sign">ǩ���ִ�</param>
        /// <param name="mobileType">�ֻ�����</param>
        /// <returns></returns>
        [HttpGet]

        public BaseResponseModel Login(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string retialUser, string retianToken, string sign, int mobileType = -1)
        {

            /*
                �������
                MobileType�����ش���Int���ͣ��ֻ�����
                Pid�����ش���String���ͣ���ȡ�˺Žӿ��·����˺ţ���������¼����Ϊ��
                Pwd�����ش���String���ͣ���ȡ�˺Žӿ��·������룬��������¼����Ϊ��
                IMEI��String���ͣ��ֻ��豸ʶ���룬����Ϊ�գ���Ϊ��ʱ����ϴ���ע����������ϴ�ʹ�õ��˺�
                ScreenX��Int���ͣ��ֻ���Ļ���
                ScreenY��Int���ͣ��ֻ���Ļ�߶�
                RetailID��String���ͣ��ͻ������������ţ��������ͻ���Э��ȷ��
                IsCustom��Bool���ͣ��˺�ʹ���Զ���ģ�TrueΪ���ã�
                RetialUser:String���ͣ���������¼���˺�ID
                RetialToken:String���ͣ���������¼���˺����ƾ֤
                ��Ӧ����
                Token��String���ͣ���¼��Ȩ�ɹ���õ�ƾ֤����Ч�ڣ�24H��
                UserID��GUID���û���Ψһ���
                ʾ��
                http://127.0.0.1:80/Login?MobileType=0&Pid=Z17465&Pwd=564613&IMEI=00:02:32:65&ScreenX=&ScreenY=&RetailID=&sign=c792a4eb8a7761524ea6e512f0efc939
             */

            #region �ǿ���֤
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(pwd) || mobileType < 0)
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Login),
                    StateCode = (int)StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region ��֤Sign
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Login),
                    StateCode = (int)StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region ��֤�˺�����
            if (!CheckId(pid, pwd))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Login),
                    StateCode = (int)StateCode.PassworkError,
                    StateDescription = DefineCode.PasswordOrPassError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }

            #endregion

            #region ִ��DB����
            Guid token = Guid.NewGuid();
            Task tokenTask = Task.Run(() => InsertTokenList(token,pid)); //д��token�б�
            Task logTask = Task.Run(() => InsertLoginLog(pid, imei, retialUser, retianToken));
            #endregion

            return new BaseResponseModel()
            {
                Handler = nameof(AccountController.Login),
                Data = new { Token = token }
            };
        }


        /// <summary>
        /// ����˺������Ƿ���ȷ
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private bool CheckId(string pid, string pwd)
        {
            //���� pid
            //if ()


            //��֤���ܺ�������Ƿ�һ��
            if (true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// д���½��־
        /// </summary>
        /// <param name="userId">�˺�</param>
        /// <param name="imei">�豸��</param>
        /// <param name="retialUser">�������˺�</param>
        /// <param name="retianToken">������token</param>
        private void InsertLoginLog(string userId, string imei, string retialUser, string retianToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// һ��ע�� �Զ�ע��ӿ�
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