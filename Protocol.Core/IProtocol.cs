
using Communication.Core;

namespace Protocol.Core
{
    /// <summary>
    /// 串口报文处理
    /// </summary>
    /// <param name="isSend">true:发送 false:接收</param>
    /// <param name="msg">报文</param>
    public delegate void MsgHander(bool isSend, string msg);

    public abstract class IProtocol
    {
        //声明报文处理事件
        public static event MsgHander MsgEvent;

        protected static string _portName = "COM1";
        protected static int _baudRate = 2400;
        private readonly string _4FE = "FEFEFEFE";

        /// <summary>
        /// 串口号
        /// </summary>
        public static string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                if (value.Substring(0, 3).ToUpper() == "COM")
                {
                    _portName = value;
                }
            }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public static int BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                if (value > 0)
                {
                    _baudRate = value;
                }
            }
        }

        protected SerialPortCom _sp = null;

        /// <summary>
        /// 构造函数
        /// 初始化串口号，波特率，判断读完事件
        /// </summary>
        public IProtocol()
        {
            _sp = new SerialPortCom(_portName, _baudRate);
            _sp.ReceiveFinishEven += ReceiveFinishJudge;
        }

        /// <summary>
        /// 判断是否读完
        /// </summary>
        /// <param name="frm">报文</param>
        /// <returns>bool类型</returns>
        protected abstract bool ReceiveFinishJudge(byte[] frm);

        /// <summary>
        /// 分析报文
        /// </summary>
        /// <param name="frm">报文</param>
        protected abstract void AnayzeFrm(string frm);

        protected virtual bool Send(string str)
        {
            str = _4FE + str;
            if(MsgEvent != null)
            {
                MsgEvent(true, str);
            }
            return _sp.Send(str);
        }

        protected virtual string SendAndRec(string str)
        {
            str = _4FE + str;
            if(MsgEvent != null)
            {
                MsgEvent(true,str);
            }
            string ret = _sp.SendAndReceive(str);

            AnayzeFrm(ret);

            if (MsgEvent != null)
            {
                MsgEvent(false, ret);
            }
            return ret;
        }

        
    }
}
