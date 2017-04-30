using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    class WebSocketHandler
    {
        private readonly WebSocketServer _ws;
        private readonly Int32 _port;
        public WebSocketHandler(int port = 5088)
        {
            _ws = new WebSocketServer();
            _port = port;   //TODO： 作为配置文件来读取  暂时写死
            _ws.NewSessionConnected += OnClientConnected;
            _ws.NewMessageReceived += OnMessageReceived;
            _ws.NewDataReceived += OnDataReceived;
            _ws.SessionClosed += OnClientClosed;

        }

        public void Start()
        {
            if (_ws.Setup(_port)) _ws.Start();
        }


        /// <summary>
        /// 客户端断开
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        public void OnClientClosed(WebSocketSession session, CloseReason value)
        {
            //TODO：客户端断开处理

        }

        /// <summary>
        /// 传入客户端数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        public void OnDataReceived(WebSocketSession session, byte[] value)
        {


            //TODO: 收到二进制数据处理
        }

        /// <summary>
        /// 收到客户端消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>

        public void OnMessageReceived(WebSocketSession session, string value)
        {
            var sid = session.SessionID;
            //value 中分离出来 各种参数
            //NameValueColl
            //TODO: 收到文本消息处理
            //分发消息
        }


        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="session"></param>
        public void OnClientConnected(WebSocketSession session)
        {

            //TODO 客户端连接处理
            //预处理
        }
    }
}
