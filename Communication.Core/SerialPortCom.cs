using System;
using System.IO.Ports;

namespace Communication.Core
{
    /// <summary>
    /// 判断是否读完
    /// </summary>
    /// <param name="data">报文</param>
    /// <returns>true:读完 false:未读完</returns>
    public delegate bool ReceiveFinishHandler(byte[] data);

    public class SerialPortCom
    {
        //声明接收判断是否读完事件
        public event ReceiveFinishHandler ReceiveFinishEven;

        //声明串口
        private SerialPort _serialPort = null;

        /// <summary>
        /// 定义最大帧字节数
        /// </summary>
        private const int MAXFRAMLEN = 1024;

        #region 串口号
        private string _portName = "COM1";
        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName
        {
            get { return _serialPort.PortName; }
            set
            {
                _serialPort.PortName = value;
                _portName = value;
            }
        }
        #endregion

        #region 波特率
        private int _baudRate = 2400;
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate
        {
            get { return _serialPort.BaudRate; }
            set
            {
                _serialPort.BaudRate = value;
                _baudRate = value;
            }
        }
        #endregion

        #region 构造与析构
        /// <summary>
        /// 默认构造函数，操作COM1，速度为9600，没有奇偶校验，8位字节，停止位为1 "COM1", 9600, Parity.None, 8, StopBits.One
        /// </summary>
        public SerialPortCom()
        {
            _serialPort = new SerialPort();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="comPort">串口号</param>
        /// <param name="baudRate">波特率</param>
        public SerialPortCom(string comPort, int baudRate)
        {
            _serialPort = new SerialPort(comPort, baudRate, Parity.Even, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.RtsEnable = true;
            _serialPort.ReadTimeout = 2000;
            _serialPort.WriteTimeout = 2000;
            _serialPort.ReadBufferSize = 1024;
            _serialPort.WriteBufferSize = 1024;
        }

        /// <summary>
        /// 构造函数,可以自定义串口的初始化参数
        /// </summary>
        /// <param name="comPortName">需要操作的COM口名称</param>
        /// <param name="baudRate">COM的速度</param>
        /// <param name="parity">奇偶校验位</param>
        /// <param name="dataBits">数据长度</param>
        /// <param name="stopBits">停止位</param>
        public SerialPortCom(string comPortName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(comPortName, baudRate, parity, dataBits, stopBits);
            _serialPort.Handshake = Handshake.None;
            _serialPort.RtsEnable = true;
            _serialPort.ReadTimeout = 2000;
            _serialPort.WriteTimeout = 2000;
            _serialPort.ReadBufferSize = 1024;
            _serialPort.WriteBufferSize = 1024;
        }

        ~SerialPortCom()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort = null;
            }
        }
        #endregion

        #region 打开串口
        /// <summary>
        /// 打开串口资源
        /// <returns>返回bool类型</returns>
        /// </summary>
        public bool OpenPort()
        {
            bool ok = false;

            //如果串口是打开的，先关闭
            if (_serialPort.IsOpen)
                _serialPort.Close();

            try
            {
                //打开串口
                _serialPort.Open();
                ok = true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

            return ok;
        }
        #endregion

        #region 关闭串口
        /// <summary>
        /// 关闭串口资源,操作完成后,一定要关闭串口
        /// </summary>
        public void ClosePort()
        {
            //如果串口处于打开状态,则关闭
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
        }
        #endregion

        #region 获取当前全部串口资源
        /// <summary>
        /// 获得当前电脑上的所有串口资源
        /// </summary>
        /// <returns></returns>
        public string[] GetSerials()
        {
            return SerialPort.GetPortNames();
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送字符串数据
        /// </summary>
        /// <param name="txString">要发送的数据</param>
        /// <returns>true:成功 false:失败</returns>
        public bool Send(string strData)
        {
            if (!OpenPort())
            {
                return false;
            }

            try
            {
                ComWriteString(strData);
            }
            catch (Exception ex)
            {
                _serialPort.DiscardOutBuffer();
                throw ex;
            }
            finally
            {
                ClosePort();
            }
            return true;
        }

        /// <summary>
        /// 发送字节数据
        /// </summary>
        /// <param name="sendBytes">要发送的字节数据</param>
        /// <returns>true:成功 false:失败</returns>
        public bool Send(byte[] byteData)
        {
            if (!OpenPort())
            {
                return false;
            }

            try
            {
                _serialPort.Write(byteData, 0, byteData.Length);
            }
            catch (Exception ex)
            {
                _serialPort.DiscardOutBuffer();
                throw ex;
            }
            finally
            {
                ClosePort();
            }
            return true;
        }

        /// <summary>
        /// 串口写String
        /// </summary>
        /// <param name="str">string类型数据</param>
        private void ComWriteString(string str)
        {
            byte[] data = StrToByte(str);
            try
            {
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 接收数据
        /// <summary>
        /// 串口接收数据
        /// </summary>
        /// <returns>接收到的数据</returns>
        public string Receive()
        {
            if (!OpenPort())
            {
                return null;
            }
            string ret = null;
            byte[] buffer = new byte[MAXFRAMLEN];
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            try
            {
                long lngTicks = DateTime.Now.Ticks;
                while (DateTime.Now.Ticks - lngTicks < 2000 * 10000)        //2秒
                {
                    System.Threading.Thread.Sleep(80);
                    if (_serialPort.BytesToRead > 0)
                    {
                        int recv = _serialPort.Read(buffer, 0, _serialPort.BytesToRead);
                        //写入内存流
                        stream.Write(buffer, 0, recv);

                        //判断是否读完
                        if (ReceiveFinishEven != null)
                        {
                            if (ReceiveFinishEven(stream.ToArray()))
                            {
                                break;
                            }
                        }
                    }
                }
                ret = ByteToHexStr(stream.ToArray());
            }
            catch (Exception ex)
            {
                _serialPort.DiscardInBuffer();
                throw ex;
            }
            finally
            {
                ClosePort();
            }

            return ret;
        }
        #endregion

        #region 发送并接收
        /// <summary>
        /// 发送并接收
        /// </summary>
        /// <param name="strData">发送string类型数据</param>
        /// <returns>接收到的数据</returns>
        public string SendAndReceive(string strData)
        {
            if (!OpenPort())
            {
                return null;
            }

            string ret = null;
            try
            {
                //发送
                ComWriteString(strData);

                //接收
                byte[] buffer = new byte[MAXFRAMLEN];
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                long lngTicks = DateTime.Now.Ticks;
                while (DateTime.Now.Ticks - lngTicks < 2000 * 10000)        //2秒
                {
                    System.Threading.Thread.Sleep(80);
                    if (_serialPort.BytesToRead > 0)
                    {
                        int recv = _serialPort.Read(buffer, 0, _serialPort.BytesToRead);
                        //写入内存流
                        stream.Write(buffer, 0, recv);

                        //判断是否读完
                        if (ReceiveFinishEven != null)
                        {
                            if (ReceiveFinishEven(stream.ToArray()))
                            {
                                break;
                            }
                        }
                    }
                }
                ret = ByteToHexStr(stream.ToArray());
            }
            catch (Exception ex)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                throw ex;
            }
            finally
            {
                ClosePort();
            }
            return ret;
        }


        #endregion

        #region 数据格式转换
        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str">string类型数据</param>
        /// <returns>byte[]类型数据</returns>
        private byte[] StrToByte(string str)
        {
            str = str.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            int len = str.Length;

            if ((len & 0x01) == 0x01)//如果长度为奇数，去掉最后一个字符
            {
                len--;
            }

            byte[] buffer = new byte[len / 2];
            for (int i = 0; i < len; i += 2)
            {
                buffer[i / 2] = System.Convert.ToByte(str.Substring(i, 2), 16);
            }
            return buffer;
        }

        /// <summary>
        /// 字节数组转hex字符串
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>hex字符串</returns>
        private string ByteToHexStr(byte[] b)
        {
            string str = string.Empty;
            if (b != null)
            {
                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = (byte)(b[i] % 256);
                    str += b[i].ToString("X2");
                }
            }
            return str;
        }
        #endregion
    }

}
