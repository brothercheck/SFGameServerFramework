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
        #region ����ע��
        private readonly IAccountInfoRepository _accountInfoRep;
        private readonly IUserInfoRepository _userInfoRep;
        private readonly ITokenListRepository _tokenListRep;
        private readonly IRegisterLogRepository _regLogRep;
        private readonly ILoginLogRepository _loginLogRep;
        public AccountController()
        {
            _accountInfoRep = new AccountInfoRepository();
            _userInfoRep = new UserInfoRepository();
            _tokenListRep = new TokenListRepository();
            _regLogRep = new RegisterLogRepository();
            _loginLogRep = new LoginLogRepository();
        }
        #endregion

        #region �˺�ע�� [HttpGet]   Register(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string sign, byte mobileType = 0, bool isCustom = true)
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
                    StateCode = StateCode.ParamsError,
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
                    StateCode = StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region ִ��
            //TODO:
            string token = Guid.NewGuid().ToString().Replace("-", "");
            Guid userId = Guid.NewGuid();
            BaseResponseModel result = new BaseResponseModel();
            bool tag = false;
            try
            {
                var regTask = Register(pid, pwd, mobileType, imei, HttpContext.Connection.RemoteIpAddress.ToString());       //ע���˺�
                Task tokenTask = Task.Run(() => InsertTokenList(token, pid));  //����token ��д��Token�б�
                Task logTask = Task.Run(() => InsertRegLog(pid, imei, mobileType));//д����־
                Task loginTask = Task.Run(() => InsertLoginLog(pid, imei));  //д���½��־
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

        #region д��ע����־ InsertRegLog(string pid, string imei, byte mobileType)
        /// <summary>
        /// д��ע����־
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="imei"></param>
        /// <param name="mobileType"></param>
        private void InsertRegLog(string pid, string imei, byte mobileType)
        {
            _regLogRep.Add(new RegisterLog(pid, mobileType, imei));

            //TODO
            //�쳣����
        }
        #endregion

        #region д��tokenList InsertTokenList(string token, string userId)
        /// <summary>
        /// д��Token���ݵ�����
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userId">userid</param>
        private void InsertTokenList(string token, string userId)
        {
            //���userid �Ƿ��Ѵ���
            bool isExists = _tokenListRep.ExistsName(userId);
            //���� ����token
            if (isExists)
            {
                var isExistsToken = _tokenListRep.SingleByName(userId);
                isExistsToken.Update(token);
                _tokenListRep.Save(isExistsToken);
            }
            else
            {
                //�����ڣ����� userid ��token
                TokenList tokenList = new TokenList(userId, token);
                _tokenListRep.Add(tokenList);
            }
            //TODO
            //�쳣����
        }
        #endregion

        #region ע�����û�д��db Register(string pid, string pwd, byte mobileType, string imei, string registerIp)
        /// <summary>
        /// ע�����û��˺�
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private int Register(string pid, string pwd, byte mobileType, string imei, string registerIp)
        {
            //����˺��Ƿ��ظ�
            if (_accountInfoRep.ExistsName(pid))
            {
                return -1;
            }
            bool arg = false;
            //д���ɫ��  �����½�ɫ
            AccountInfo account = new AccountInfo(pid)
            {
                Password = pwd,
                IMEI = imei,
                RegisterIp = registerIp
            };
            try
            { //�������ݿ�
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
        #endregion

        #region �û���¼ Login(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string retialUser, string retialToken, string sign, int mobileType = -1)      
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
        /// <param name="retialToken">������token</param>
        /// <param name="sign">ǩ���ִ�</param>
        /// <param name="mobileType">�ֻ�����</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponseModel Login(string pid, string pwd, string imei, int screenX, int screenY, string retailId, string retialUser, string retialToken, string sign, int mobileType = -1)
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
                    StateCode = StateCode.ParamsError,
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
                    StateCode = StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion

            #region ��֤�˺�����
            switch (CheckId(pid, pwd, imei))
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
                case -2:
                    return new BaseResponseModel()
                    {
                        Handler = nameof(AccountController.Login),
                        StateCode = StateCode.PasswordError,
                        StateDescription = DefineCode.PassportError,
                        Vesion = DefineCode.Version,
                        Data = null
                    };
            }

            #endregion

            #region ִ��DB����
            string token = Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                Task tokenTask = Task.Run(() => InsertTokenList(token, pid)); //д��token�б�
                Task logTask = Task.Run(() => InsertLoginLog(pid, imei, retialUser, retialToken));  //д����־
            }
            catch (Exception ex)
            {
                //TODO:�쳣����  �˺ŵ�½
                return new BaseResponseModel()
                {
                    StateCode = StateCode.LoginError,
                    StateDescription = ex.Message,
                    Handler = nameof(AccountController.Login),
                    Data = null
                };
            }
            #endregion

            return new BaseResponseModel()
            {
                Handler = nameof(AccountController.Login),
                Data = new { Token = token }
            };
        }
        #endregion

        #region ����˺������Ƿ���Ч CheckId(string pid, string pwd, string imei)
        /// <summary>
        /// ����˺������Ƿ���ȷ
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private int CheckId(string pid, string pwd, string imei)
        {
            //���� pid
            if (_accountInfoRep.ExistsName(pid))
            {//�˺Ŵ���,��֤����
                var user = _accountInfoRep.SingleByName(pid);
                if (!user.Password.Equals(pwd))
                {   //���벻��
                    if (!user.Name.Equals(imei))
                    {   //����imeiҲ�Բ���
                        return -1;
                    }
                }
                return 0;
            }
            else
            {   //d�˺Ų�����
                return -2;
            }
        }
        #endregion

        #region д���½��־ InsertLoginLog(string userId, string imei, string retialUser = "", string retialToken = "")
        /// <summary>
        /// д���½��־
        /// </summary>
        /// <param name="userId">�˺�</param>
        /// <param name="imei">�豸��</param>
        /// <param name="retialUser">�������˺�</param>
        /// <param name="retialToken">������token</param> 
        private void InsertLoginLog(string userId, string imei, string retialUser = "", string retialToken = "")
        {
            LoginLog loginLog = new LoginLog(userId, imei, retialUser, retialToken);
            _loginLogRep.Add(loginLog);
        }
        #endregion

        #region һ��ע�� Passport(string imei, string sign, byte mobileType = 0)
        /// <summary>
        /// һ��ע�� �Զ�ע��ӿ�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponseModel Passport(string imei, string sign, byte mobileType = 0)
        {
            //IMEI=00:02:32:65&sign=2bcc51b165368e139111914e12864a89
            #region �ǿ���֤
            if (string.IsNullOrEmpty(imei) || string.IsNullOrEmpty(sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Passport),
                    StateCode = StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }

            #endregion

            #region ��֤sign
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

            #region ִ��
            var token = Guid.NewGuid().ToString().Replace("-", "");
            if (_accountInfoRep.ExistsName(imei))
            {
                //imei���ڣ�ֱ�ӷ���token �� userid
                var user = _accountInfoRep.SingleByName(imei);
                //д�� tokenlist
                Task tokenTask = Task.Run(() => InsertTokenList(token, imei));  //����token ��д��Token�б�
                Task loginTask = Task.Run(() => InsertLoginLog(imei, imei));  //д���½��־
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Passport),
                    Data = new { Token = token, UserId = user.Name }
                };
            }
            else
            {
                //�����ڣ�ע�����˺�
                AccountInfo accountInfo = new AccountInfo(imei);
                _accountInfoRep.Add(accountInfo);
                Task logTask = Task.Run(() => InsertRegLog(imei, imei, mobileType));//д��ע����־
                Task tokenTask = Task.Run(() => InsertTokenList(token, imei));  //����token ��д��Token�б�
                Task loginTask = Task.Run(() => InsertLoginLog(imei, imei));  //д���½��־
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Passport),
                    Data = new { Token = token, UserId = accountInfo.Name }
                };
            }
            #endregion
        }
        #endregion

        #region �޸�����ӿ� Password(string passportId, string password, string sign)
        [HttpGet]
        public BaseResponseModel Password(string passportId, string password, string sign)
        {
            #region �ǿ���֤
            if (string.IsNullOrEmpty(passportId) || string.IsNullOrEmpty(passportId) || string.IsNullOrEmpty(sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Password),
                    StateCode = StateCode.ParamsError,
                    StateDescription = DefineCode.ParamsError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion
            #region ��֤sign
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Password),
                    StateCode = StateCode.SignError,
                    StateDescription = DefineCode.SignError,
                    Vesion = DefineCode.Version,
                    Data = null
                };
            }
            #endregion
            #region �޸�����
            //1�����û�
            var user = _accountInfoRep.SingleByName(passportId);
            if (user != null)
            {
                user.Password = password;
                _accountInfoRep.Save(user);
                return new BaseResponseModel()
                {
                    Handler = nameof(AccountController.Password),
                    Data = null
                };
            }
            else
            {
                //�޴��û�
                return new BaseResponseModel()
                {
                    StateCode = StateCode.PassportError,
                    StateDescription = DefineCode.PassportError,
                    Vesion = DefineCode.Version,
                    Handler = nameof(AccountController.Password),
                    Data = null
                };
            }
            #endregion
        }
        #endregion
    }
}