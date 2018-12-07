using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Common;

namespace Protocol.Core
{
    public class Protocol645:IProtocol
    {
        #region 表地址
        private static string _addr;
        /// <summary>
        /// 电表地址
        /// （12个字符）
        /// </summary>
        public static string Addr
        {
            get
            {
                return _addr;
            }
            set
            {
                if (value.Length <= 12 && value.Length > 0)
                {
                    _addr = value.PadLeft(12, '0');//不足12个字符前面补0
                }
                else
                {
                    _addr = value;
                }

            }
        }
        #endregion

        #region 操作者代码
        private static string _oprCode;
        /// <summary>
        /// 操作者代码
        /// （8个字符）
        /// </summary>
        public static string OprCode
        {
            get
            {
                return _oprCode;
            }
            set
            {
                if (value.Length <= 8)
                {
                    _oprCode = value.PadLeft(8, '0');//不足8个字符前面补0
                }
            }
        }
        #endregion

        #region 密码
        private static string _psw;
        /// <summary>
        /// 电表密码
        /// （8个字符）
        /// </summary>
        public static string Psw
        {
            get
            {
                return _psw;
            }
            set
            {
                if(value.Length <= 8)
                {
                    _psw = value.PadLeft(8, '0');//不足8个字符前面补0
                }
            }
        }
        #endregion

        private Frame645 _retFrm = null;

        #region 构造与析构
        public Protocol645()
        {

        }

        ~Protocol645()
        {
            if (_sp != null)
            {
                _sp.ClosePort();
            }
        }
        #endregion
        /// <summary>
        /// 判断是否读完
        /// </summary>
        /// <param name="frm">输入帧</param>
        /// <returns></returns>
        protected override bool ReceiveFinishJudge(byte[] frm)
        {
            Frame645 frm645 = new Frame645(frm);
            return frm645.IsValidFrame;
        }

        /// <summary>
        /// 分析报文
        /// </summary>
        /// <param name="frm"></param>
        protected override void AnayzeFrm(string frm)
        {
            _retFrm = null;
            _retFrm = new Frame645(frm);
        }

        /// <summary>
        /// 判断ID长度是否为8字符
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <returns>bool类型</returns>
        private bool JudgeId(string dataID)
        {
            return dataID.Length == 8;
        }

        #region ASCII码标识符
        private readonly string[] ascii = new string[]{
            "04000403", //资产管理编码            
            "04000404", //额定电压
            "04000405", //额定电流
            "04000406", //最大电流
            "04000407", //有功准确度等级
            "04000408", //无功准确度等级
            "0400040B", //电表型号
            "0400040C", //生产日期
            "0400040D", //协议版本号
            "04800001", //厂家软件版本号
            "04800002", //厂家硬件版本号
            "04800003", //厂家编号
            "048000E1", //科美内部软件版本号
        };
        #endregion

        #region 数据组帧
        private string AssembleFrm(string ctl, string len, string data)
        {
            string frm = string.Empty;
            frm += "68";
            frm += Transfer.ReverseString(_addr);
            frm += "68";
            frm += ctl;
            frm += len;
            frm += Counter.StringAdd33(data);
            frm += Counter.Sum(frm);
            frm += "16";
            return frm;
        }
        #endregion

        private bool SendAndRecCustom(string txString, out string ctl, out string result)//有返回数据
        {
            result = null;
            ctl = null;

            string ret = SendAndRec(txString);

            if (ret != null)
            {
                _retFrm = new Frame645(ret);
                if (!_retFrm.IsValidFrame) return false;
                ctl = _retFrm.CtrolNum;
                result = _retFrm.OriginalData;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region 读数据

        /// <summary>
        /// 控制码11读数据
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <param name="result">返回数据</param>
        /// <returns>true/false</returns>
        public bool ReadData(string dataID, out string result)
        {
            //控制码：C=11H
            //数据域长度：L=04H+m
            //帧格式1（m=0）：68 A0 ... A5 68 11 04 DI0 ... DI3 CS 16
            //帧格式2（m=1，读给定块数的负荷记录）：68 A0 ... A5 68 11 05 DI0 ... DI3 N CS 16
            //帧格式3（m=6，读给定时间、块数的负荷纪录）：68 A0 ... A5 68 11 0A DI0 ... DI3 N mm hh DD MM YY CS 16
            result = null;
            if (!JudgeId(dataID))
            {
                return false;
            }

            string sendFrm = AssembleFrm("11", "04", Transfer.ReverseString(dataID));
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x91 || _retFrm.hexCtrolNum == 0xB1)
                {
                    result = _retFrm.Data;
                    if (ascii.Contains(dataID))
                    {
                        result = Transfer.AsciiToString(result);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 读数据块
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <param name="ctl">返回控制码</param>
        /// <param name="result">返回数据</param>
        /// <returns>true/false</returns>
        public bool ReadBlockData(string dataID, out byte ctl, out string result)
        {
            result = null;
            ctl = 0;
            if (!JudgeId(dataID)) return false;

            string sendFrm = AssembleFrm("11", "04", Transfer.ReverseString(dataID));
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                ctl = _retFrm.hexCtrolNum;
                if (ctl == 0x91 || ctl == 0xb1)
                {
                    result = _retFrm.Data;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 读后续数据
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <param name="seq">帧序号</param>
        /// <param name="ctl">返回控制码</param>
        /// <param name="result">返回数据</param>
        /// <returns>true/false</returns>
        public bool ReadNextBlock(string dataID, byte seq, out byte ctl, out string result)
        {
            result = null;
            ctl = 0;
            if (!JudgeId(dataID)) return false;

            string sendFrm = AssembleFrm("12", "05", (Transfer.ReverseString(dataID) + seq.ToString("X2")));
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                ctl = _retFrm.hexCtrolNum;
                if (ctl == 0x92 || ctl == 0xB2)
                {
                    result = _retFrm.Data;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 读参数
        /// </summary>
        /// <param name="tbObj">文本框控件name</param>
        /// <param name="dataId">数据ID</param>
        public bool ReadParameter(object tbObj, string dataId)
        {
            if (!(tbObj is TextBox)) return false;

            TextBox tb = (TextBox)tbObj;
            tb.ForeColor = Color.Black;
            tb.Text = "";
            Functions.Delay(10);
            string result = null;
            if (ReadData(dataId, out result))
            {
                tb.Text = result;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 写数据
        /// <summary>
        /// 14写数据
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <param name="data">写数据:允许写入空“”</param>
        /// <returns>true/false</returns>
        public bool WriteData(string dataID, string data)
        {
            //控制码：C=14H
            //数据域长度：L=04H(数据标识)+04H(密码)+04H(操作者代码)+m(数据长度)
            //帧格式：68 A0 ... A5 68 14 L DI0 ... DI3 PA P0 P1 P2 C0 ... C3 N1 ...Nm CS 16
            string sData = null;
            if (!JudgeId(dataID))
            {
                return false;
            }

            data = data.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);//去掉数据里的空格、回车
            string dataTable = "0123456789ABCDEFabcdef";
            if (data != null && data.Length > 0)
            {
                foreach (char str in data)//过滤非法字符
                {
                    if (!dataTable.Contains(str))
                    {
                        MessageBox.Show("数据包含非法字符！");
                        return false;
                    }
                }
            }

            sData += Transfer.ReverseString(dataID);
            sData += Transfer.ReverseString(Psw);
            sData += Transfer.ReverseString(OprCode);
            if (data != "")
            {
                sData += Transfer.ReverseString(data);     //钜泉参数初始化不需要写入data
            }

            string sendFrm = AssembleFrm("14", (04 + 04 + 04 + data.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x94)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 写校表参数
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <param name="data">数据</param>
        /// <returns>true/false</returns>
        public bool WriteDataPro(string dataID, string data)//校表参数写
        {
            string sData = null;
            if (!JudgeId(dataID)) return false;

            sData += Transfer.ReverseString(dataID);
            if (data != "")
            {
                sData += Transfer.ReverseString(data);     //钜泉参数初始化不需要写入data
            }

            string sendFrm = AssembleFrm("0E", (04 + data.Length / 2).ToString("X2"), sData);

            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x8E)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设参数
        /// </summary>
        /// <param name="tbObj">要输入数据的文本框控件name</param>
        /// <param name="dataId">ID</param>
        public void SetParameter(object tbObj, string dataId)
        {
            if (!(tbObj is TextBox)) return;

            TextBox tb = (TextBox)tbObj;
            string strDate = tb.Text;

            if (strDate.Length != tb.MaxLength)
            {
                MessageBox.Show("数据长度过长或不足！");
                return;
            }

            if (dataId == "04000101" || strDate.Length == 6)//如果是日期，则要判断星期
            {
                strDate = Functions.JustWeek(strDate);
            }
            if (!WriteData(dataId, strDate))
            {
                tb.ForeColor = Color.Red;
                tb.Text = "Err";
            }
        }

        /// <summary>
        /// 设参数
        /// </summary>
        /// <param name="dataId">数据ID</param>
        /// <param name="data">数据</param>
        public void SetParameter(string dataId, string data)
        {
            data = data.Replace(" ", "").Replace("\r\n", "");
            WriteData(dataId, data);
        }

        #endregion

        #region 安全认证命令
        /// <summary>
        /// 安全认证命令
        /// 控制码：C=03H
        /// 数据域长度：L=04H+04H+m
        /// 帧格式1（m=0）：68 A0 ... A5 68 03 L DI0 ... DI3 C0 ... C3 N1 ... Nm CS 16
        /// </summary>
        /// <param name="dataID">数据ID</param>
        /// <param name="C0C1C2C3">操作者代码</param>
        /// <param name="putData">数据</param>
        /// <param name="result">返回数据</param>
        /// <returns>是否成功</returns>
        public bool SecurityAuthentication(string dataID, string putData, out string result)
        {
            string sData = null;
            result = null;
            if (!JudgeId(dataID)) return false;

            sData += Transfer.ReverseString(dataID);
            sData += Transfer.ReverseString(OprCode);
            if (putData.Length > 0)//使身份认证失效不需输入数据
                sData += Transfer.ReverseString(putData);
            string sendFrm = AssembleFrm("03", (4 + 4 + putData.Length / 2).ToString("X2"), sData);

            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x83)
                {
                    result = _retFrm.Data;
                    return true;
                }
                else if (_retFrm.hexCtrolNum == 0xC3)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 福建点对点校时
        public bool FujianDianDuiDianSetTime(string data)
        {
            string sendFrm = AssembleFrm("08", "06", Transfer.ReverseString(data));
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x88)
                {
                    return true;
                }
                else if (_retFrm.hexCtrolNum == 0xC8)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 广播校时
        public bool BroadcastSetTime(string dataOfTime)
        {
            string tmpAddr = _addr;
            _addr = "999999999999";
            string sendFrm = AssembleFrm("08", "06", Transfer.ReverseString(dataOfTime));
            _addr = tmpAddr;

            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region KM清EEP
        /// <summary>
        /// 单相表清EEPROM
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool KMClrEEP(out string result)
        {
            string sData = null;
            sData += Transfer.ReverseString("CAFFFFFF");
            sData += Transfer.ReverseString("12345678");

            string ctl = null;
            string revData = null;
            result = null;
            string sendFrm = AssembleFrm("CA", "08", sData);
            if (SendAndRecCustom(sendFrm, out ctl, out revData))
            {
                if (revData == null)
                {
                    return true;
                }
                if (revData == "04")
                {
                    result = revData;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public bool KM3PhaseClrEEP(string putVol, string putCurr, string putMaxCurr, string putYouGongLevel, string putWuGongLevel,
            string putYouGongConst, string putWuGongConst, string putMeterModel, string putProductDate, string putProtocalVer)
        {
            string sData = null;
            sData += Transfer.ReverseString(putVol);
            sData += Transfer.ReverseString(putCurr);
            sData += Transfer.ReverseString(putMaxCurr);
            sData += Transfer.ReverseString(putYouGongLevel);
            sData += Transfer.ReverseString(putWuGongLevel);
            sData += Transfer.ReverseString(putYouGongConst);
            sData += Transfer.ReverseString(putWuGongConst);
            sData += Transfer.ReverseString(putMeterModel);
            sData += Transfer.ReverseString(putProductDate);
            sData += Transfer.ReverseString(putProtocalVer);

            string mFrm = AssembleFrm("22", "1C", sData);

            if (Send(mFrm))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 复旦微命令
        /// <summary>
        /// 复旦微特殊写命令
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool FDWWriteData(string dataID, string data)
        {
            string sData = "";
            if (!JudgeId(dataID)) return false;

            sData += Transfer.ReverseString(dataID);
            sData += Transfer.ReverseString(data);

            string sendFrm = AssembleFrm("1F", (02 + data.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9F)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 复旦微特殊广播命令
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool FDWBroad(string dataID, string data, out string result)
        {
            string sData = string.Empty;
            result = null;
            if (dataID.Length < 4)
            {
                MessageBox.Show("ID不足4位，请重新输入！");
                return false;
            }

            sData += Transfer.ReverseString(dataID);
            sData += Transfer.ReverseString(data);

            string sendFrm = AssembleFrm("1F", (02 + data.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9F)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 更改通讯波特率
        public bool ChangeBaudrate(string baudrate)
        {
            string sendFrm = AssembleFrm("17", "01", baudrate);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x97)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 读写通讯地址
        //控制码：C=13H
        //数据域长度：L=00H
        //帧格式：68 AA ... AA 68 13 00 CS 16
        public bool ReadAddress(out string result)
        {
            result = null;
            string ret = SendAndRec("68AAAAAAAAAAAA681300DF16");
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x93)
                {
                    result = _retFrm.MeterAddress;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        //控制码：C=15H
        //数据域长度：L=06H
        //帧格式：68 AA ... AA 68 15 06 A0 ... A5 CS 16
        public bool WriteAddress(string Addr)
        {
            string tmpAddr = _addr;
            _addr = "AAAAAAAAAAAA";
            string sendFrm = AssembleFrm("15", "06", Transfer.ReverseString(Addr));
            _addr = tmpAddr;

            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x95)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 电表清零
        //控制码：C=1BH
        //数据域长度：L=0CH
        //电表清零帧格式：  68 A0 ... A5 68 1A 08 PA P0 P1 P2 C0 ... C3 CS 16
        public bool MeterClr(/*out string result*/)
        {
            string sData = "";
            sData += Transfer.ReverseString(Psw);
            sData += Transfer.ReverseString(OprCode);

            string sendFrm = AssembleFrm("1A", "08", sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9a)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool MeterClr(string putData /*, out string result*/)
        {
            string sData = "";
            sData += Transfer.ReverseString(Psw.Substring(0, 6) + "98");
            sData += Transfer.ReverseString(OprCode);
            sData += Transfer.ReverseString(putData);

            string sendFrm = AssembleFrm("1A", (04 + 04 + putData.Length / 2).ToString("X2"), sData);

            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9a)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 事件清零
        //控制码：C=1BH
        //数据域长度：L=0CH
        //事件总清零帧格式：  68 A0 ... A5 68 1B 0C PA P0 P1 P2 C0 ... C3 FF FF FF FF CS 16
        //分项事件清零帧格式：68 A0 ... A5 68 1B 0C PA P0 P1 P2 C0 ... C3 FF DI1 DI2 DI3 CS 16
        public bool EventClear(string dataID = "FFFFFFFF" /*, out string result*/)  //事件清零
        {
            string sData = "";
            sData += Transfer.ReverseString(Psw);
            sData += Transfer.ReverseString(OprCode);
            if (dataID != "FFFFFFFF") sData += "FF";
            sData += Transfer.ReverseString(dataID);

            string sendFrm = AssembleFrm("1B", "0C", sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9b)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool EventClear98(string putData)  //98级密码事件清零
        {
            string sData = "";
            sData += Transfer.ReverseString(Psw.Substring(0, 6) + "98");
            sData += Transfer.ReverseString(OprCode);
            sData += Transfer.ReverseString(putData);

            string sendFrm = AssembleFrm("1B", (04 + 04 + putData.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9b)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 最大需量清零
        //控制码：C=19H
        //数据域长度：L=08H + 数据长度
        //最大需量清零帧格式：  68 A0 ... A5 68 19 08 PA P0 P1 P2 C0 ... C3 CS 16
        //98级最大需量清零帧格式：68 A0 ... A5 68 19 L PA P0 P1 P2 C0 ... C3 N1 ... Nm CS 16
        public bool MaxDemandClear()  //最大需量清零
        {
            string sData = "";
            sData += Transfer.ReverseString(Psw);
            sData += Transfer.ReverseString(OprCode);

            string sendFrm = AssembleFrm("19", "08", sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x99)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool MaxDemandClear98(string putData)  //98级最大需量清零
        {
            string sData = "";
            sData += Transfer.ReverseString(Psw.Substring(0, 6) + "98");
            sData += Transfer.ReverseString(OprCode);
            sData += Transfer.ReverseString(putData);

            string sendFrm = AssembleFrm("19", (04 + 04 + putData.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x99)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 冻结命令
        //控制码：C=16H
        //数据域长度：L=04H
        //电表清零帧格式：  68 99 ... 99 68 16 04 mm hh DD MM CS 16
        public bool BroadcastFreeze(string freezeTime)
        {
            string tmpAddr = _addr;
            _addr = "999999999999";
            string sendFrm = AssembleFrm("16", "04", Transfer.ReverseString(freezeTime));
            _addr = tmpAddr;
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 控制功能
        /// <summary>
        /// 控制码：C=1CH
        /// 数据域长度：L=04H(密码)+04H(操作者代码)+m(数据长度)
        /// 帧格式：68 A0 ... A5 68 1C L PA P0 P1 P2 C0 ... C3 N1 ...Nm CS 16
        /// </summary>
        /// <param name="N1">N1命令</param>
        /// <param name="N2">N2默认为00，当N1为预跳闸命令时，N2表示时间</param>
        /// <param name="endTime">命令有效截止时间</param>
        /// <param name="result">返回错误代码</param>
        /// <returns>成功/失败</returns>
        public bool FeikongControl(string putEndata)
        {
            string sData = null;
            sData += Transfer.ReverseString(Psw);
            sData += Transfer.ReverseString(OprCode);
            sData += Transfer.ReverseString(putEndata);

            string sendFrm = AssembleFrm("1C", (04 + 04 + putEndata.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9c)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region KM清/设出厂
        public bool Factory040001E0(bool isSetFac)
        {
            string sendFrm = null;
            if (!isSetFac)
            {
                sendFrm = AssembleFrm("14", "0E", "E00100044D4B57470000000055AA");
            }
            else if (isSetFac)
            {
                sendFrm = AssembleFrm("14", "0E", "E00100044D4B5747000000000000");
            }
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x94)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 多功能端子输出控制命令
        public bool MultFunOutPutCtrl(string data)
        {
            string sendFrm = AssembleFrm("1D", "01", data);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x9d)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 欣瑞利97协议
        public bool SF_ReadData(string dataID, out string result)
        {
            result = null;

            string sendFrm = AssembleFrm("01", "02", Transfer.ReverseString(dataID));
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                byte[] retFrm = _retFrm.hexOriginalData;
                byte[] dataFrm = retFrm.Skip(2).Take(retFrm.Length - 2).ToArray();
                result = Transfer.ReverseString(dataFrm.ToHexString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SF_SetData(string dataID, string data)
        {
            string sData = string.Empty;

            sData += Transfer.ReverseString(dataID);
            sData += Transfer.ReverseString(Psw);

            data = data.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);//去掉数据里的空格、回车
            string dataTable = "0123456789ABCDEFabcdef";
            if (data != "")
            {
                foreach (char str in data)//过滤非法字符
                {
                    if (!dataTable.Contains(str))
                    {
                        MessageBox.Show("数据包含非法字符！");
                        return false;
                    }
                }
            }

            sData += Transfer.ReverseString(data);

            string sendFrm = AssembleFrm("04", (02 + 04 + data.Length / 2).ToString("X2"), sData);
            string ret = SendAndRec(sendFrm);
            if (ret != null && ret.Length > 0)
            {
                if (_retFrm.hexCtrolNum == 0x84)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion


    }
}
