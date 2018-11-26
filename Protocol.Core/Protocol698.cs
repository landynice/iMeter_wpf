using System.Collections.Generic;
using Common;
using Communication.Core;

namespace Protocol.Core
{
    public class Protocol698:IProtocol
    {
        public static string Addr { get; set; }
        #region 校验
        private const ushort PPPINITFCS16 = 0xffff;     //Initial FCS value
        private const ushort PPPGOODFCS16 = 0xf0b8;     // Good final FCS value
        private static readonly ushort[] fcstab = new ushort[256]
        {
            0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
            0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
            0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
            0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
            0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
            0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
            0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
            0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
            0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
            0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
            0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
            0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
            0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
            0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
            0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
            0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
            0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
            0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
            0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
            0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
            0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
            0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
            0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
            0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
            0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
            0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
            0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
            0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
            0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
            0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
            0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
            0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
        };

        //Calculate a new fcs given the current fcs and the new data.

        //u16 pppfcs16(fcs, cp, len)
        //register u16 fcs;
        //register unsigned char *cp;
        //register int len;
        //{
        //ASSERT(sizeof (u16) == 2);
        //ASSERT(((u16) -1) > 0);
        //while (len--)
        //fcs = (fcs >> 8) ^ fcstab[(fcs ^ *cp++) & 0xff];
        //return (fcs);
        //}

        public static ushort pppfcs16(string frm)
        {
            frm = frm.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            int len = frm.Length/2;
            ushort fcs = PPPINITFCS16;
            byte[] data = new byte[len];
            Transfer.StrConvertToByte(frm, out data);

            foreach (byte b in data)
            {
                fcs = (ushort)((fcs >> 8) ^ fcstab[((fcs ^ b) & 0xff)]);
            }
            
            return (ushort)(fcs^0xffff);
        }

        /// <summary>
        /// 内部使用获取CS（已反转）
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        private static string GetCS(string frm)
        {
            ushort cs = pppfcs16(frm);
            string strCs = (cs & 0x00ff).ToString("X2");
            strCs += (cs >> 8).ToString("X2");
            return strCs;
        }
        //How to use the fcs
        //tryfcs16(cp, len)
        //register unsigned char *cp;
        //register int len;
        //{
        //u16 trialfcs;
        // add on output 
        //trialfcs = pppfcs16( PPPINITFCS16, cp, len );
        //trialfcs ^= 0xffff; /* complement */
        //cp[len] = (trialfcs & 0x00ff); /* least significant byte first */
        //cp[len+1] = ((trialfcs >> 8) & 0x00ff);
        // check on input 
        //trialfcs = pppfcs16( PPPINITFCS16, cp, len + 2 );
        //if ( trialfcs == PPPGOODFCS16 )
        //printf("Good FCS\n");
        //}


        //Generate a FCS-16 table.
        //Drew D. Perkins at Carnegie Mellon University.
        //Code liberally borrowed from Mohsen Banan and D. Hugh Redelmeier.
        //The FCS-16 generator polynomial: x**0 + x**5 + x**12 + x**16.

        //#define P 0x8408

        //NOTE The hex to "least significant bit" binary always causes
        //confusion, but it is used in all HDLC documents. Example: 03H
        //translates to 1100 0000B. The above defined polynomial value
        //(0x8408) is required by the algorithm to produce the results
        //corresponding to the given generator polynomial
        //(x**0 + x**5 + x**12 + x**16)

        //main()
        //{
        //register unsigned int b, v;
        //register int i;
        //printf("typedef unsigned short u16;\n");
        //printf("static u16 fcstab[256] = {");
        //for (b = 0; ; )
        //{
        //if (b % 8 == 0)
        //printf("\n");
        //v = b;
        //for (i = 8; i--; )
        //v = v & 1 ? (v >> 1) ^ P : v >> 1;
        //printf("\t0x%04x", v & 0xFFFF);
        //if (++b == 256)
        //break;
        //printf(",");
        //}
        //printf("\n};\n");
        //}
        #endregion


        /// <summary>
        /// 数据类型字典
        /// 如果字典里没有此类型则返回null
        /// </summary>
        /// <param name="dataType">数据类型代码</param>
        /// <returns>数据类型</returns>
        public static string GetDataType(int dataType)
        { 
            Dictionary<int, string> dataTypeDict = new Dictionary<int, string>();
            dataTypeDict.Add(00, "NULL");
            dataTypeDict.Add(01, "array");
            dataTypeDict.Add(02, "structure");
            dataTypeDict.Add(03, "bool");
            dataTypeDict.Add(04, "bit-string");
            dataTypeDict.Add(05, "double-long");
            dataTypeDict.Add(06, "double-long-unsigned");
            dataTypeDict.Add(09, "octet-string");
            dataTypeDict.Add(10, "visible-string");
            dataTypeDict.Add(12, "UTF8-string");
            dataTypeDict.Add(15, "integer");
            dataTypeDict.Add(16, "long");
            dataTypeDict.Add(17, "unsigned");
            dataTypeDict.Add(18, "long-unsigned");
            dataTypeDict.Add(20, "long64");
            dataTypeDict.Add(21, "long64-unsigned");
            dataTypeDict.Add(22, "enum");
            dataTypeDict.Add(23, "float32");
            dataTypeDict.Add(24, "float64");
            dataTypeDict.Add(25, "date_time");
            dataTypeDict.Add(26, "date");
            dataTypeDict.Add(27, "time");
            dataTypeDict.Add(28, "date_time_s");
            dataTypeDict.Add(80, "OI");
            dataTypeDict.Add(81, "OAD");
            dataTypeDict.Add(82, "ROAD");
            dataTypeDict.Add(83, "OMD");
            dataTypeDict.Add(84, "TI");
            dataTypeDict.Add(85, "TSA");
            dataTypeDict.Add(86, "MAC");
            dataTypeDict.Add(87, "RN");
            dataTypeDict.Add(88, "Region");
            dataTypeDict.Add(89, "Scaler_Unit");
            dataTypeDict.Add(90, "RSD");
            dataTypeDict.Add(91, "CSD");
            dataTypeDict.Add(92, "MS");
            dataTypeDict.Add(93, "SID");
            dataTypeDict.Add(94, "SID_MAC");
            dataTypeDict.Add(95, "COMDCB");
            dataTypeDict.Add(96, "RCSD");
            if(dataTypeDict.ContainsKey(dataType))
                return dataTypeDict[dataType];
            else
                return null;
        }


        /// <summary>
        /// 错误类型字典
        /// 如果字典里没有此类型则返回null
        /// </summary>
        /// <param name="darType">错误类型代码</param>
        /// <returns>错误类型</returns>
        public static string GetDARType(int darType)
        {
            Dictionary<int, string> DARTypeDict = new Dictionary<int, string>();
            DARTypeDict.Add(00, "成功");
            DARTypeDict.Add(01, "硬件失效");
            DARTypeDict.Add(02, "暂时失效");
            DARTypeDict.Add(03, "拒绝读写");
            DARTypeDict.Add(04, "对象未定义");
            DARTypeDict.Add(05, "对象接口类不符合");
            DARTypeDict.Add(06, "对象不存在");
            DARTypeDict.Add(07, "类型不匹配");
            DARTypeDict.Add(08, "越界");
            DARTypeDict.Add(09, "数据块不可用");
            DARTypeDict.Add(10, "分帧传输已取消");
            DARTypeDict.Add(11, "不处于分帧传输状态");
            DARTypeDict.Add(12, "块写取消");
            DARTypeDict.Add(13, "不存在块写状态");
            DARTypeDict.Add(14, "数据块序号无效");
            DARTypeDict.Add(15, "密码错/未授权");
            DARTypeDict.Add(16, "通信速率不能更改");
            DARTypeDict.Add(17, "年时区数超");
            DARTypeDict.Add(18, "日时段数超");
            DARTypeDict.Add(19, "费率数超");
            DARTypeDict.Add(20, "安全认证不匹配");
            DARTypeDict.Add(21, "重复充值");
            DARTypeDict.Add(22, "ESAM验证失败");
            DARTypeDict.Add(23, "安全认证失败");
            DARTypeDict.Add(24, "客户编号不匹配");
            DARTypeDict.Add(25, "充值次数错误");
            DARTypeDict.Add(26, "购电超囤积");
            DARTypeDict.Add(27, "地址异常");
            DARTypeDict.Add(28, "对称解密错误");
            DARTypeDict.Add(29, "非对称解密错误");
            DARTypeDict.Add(30, "签名错误");
            DARTypeDict.Add(31, "电能表挂起");
            DARTypeDict.Add(32, "时间标签无效");
            DARTypeDict.Add(33, "请求超时");
            DARTypeDict.Add(255, "其它");
            if(DARTypeDict.ContainsKey(darType))
                return DARTypeDict[darType];
            else
                return null;
        }


        //#region 同步发送数据
        //public static bool SendAndReceiveFrame(string txString, out string result)
        //{
        //    ComPort.ReceiveFinishEven += ReceiveFinishJudge;
        //    result = null;
        //    if (!ComPort.Send(txString))
        //    {
        //        return false;
        //    }

        //    string rec = ComPort.Receive();

        //    if (rec != "")
        //    {
        //        result = rec;
        //        return true;
        //    }
        //    else return false;
        //}

        //#endregion

        public string SendAndRecv(string tx)
        {
            return base.SendAndRec(tx);
        }
        public string ReadAddr()
        {
            string txString = "6817004345AAAAAAAAAAAA11534E0501024001020000BB0B16";
            //返回:68 21 00 C3 45 11 11 11 11 11 11 11 7D E0 85 01 02 40 01 02 00 01 09 06 11 11 11 11 11 11 00 00 7C 3A 16 
            string ret = string.Empty;
            string res = SendAndRec(txString);
            if (res != null && res.Length > 0)
            {
                Frame698 frm = new Frame698(res);
                if (frm.IsValidFrame)
                {
                    ret = frm.SaStr;
                }
            }
            return ret;
        }

        public byte[] ReadData(string OAD)
        {
            string txString = string.Empty;
            txString = "68" + "L1L2" + "43" + (Addr.Length / 2 - 1).ToString("x2") + Addr + "10";
            string hString = txString.Substring(2);
            txString += "H_CS";
            txString += "05";//读
            if (OAD.Length / 8 > 1)
            {
                txString += "02";//多个OAD读
                txString += "01";//piid
                txString += (OAD.Length / 8).ToString("X2");//个数
            }
            else
            {
                txString += "01";//单个OAD读
                txString += "01";//piid
            }
            txString += OAD;
            txString += "00";//没有时间标签
            txString += "F_CS";
            txString += "16";

            int frmLen = txString.Length / 2 - 2;
            txString = txString.Replace("L1L2", (frmLen & 0x00ff).ToString("X2") + (frmLen >> 8).ToString("X2"));

            hString = hString.Replace("L1L2", (frmLen & 0x00ff).ToString("X2") + (frmLen >> 8).ToString("X2"));
            txString = txString.Replace("H_CS", GetCS(hString));

            string fString = txString.Substring(2, txString.Length - 8);
            txString = txString.Replace("F_CS", GetCS(fString));

            byte[] ret = {};
            string res = SendAndRec(txString);
            if (res != null && res.Length > 0)
            {
                Frame698 frm = new Frame698(res);
                if (frm.IsValidFrame)
                {
                    //ret = new byte[frm.Apdu.Length];
                    ret = frm.Apdu;
                }
            }
            return ret;
        }

        #region ESAM
        private static byte[] _meterNo = new byte[8];
        public static byte[] MeterNo
        {
            get { return _meterNo; }
            set { _meterNo = value; }
        }

        private static byte[] _serialNo = new byte[8];
        public static byte[] SerialNo
        {
            get { return _serialNo; }
            set { _serialNo = value;}
        }

        private static int _asctr;
        public static int ASCTR
        {
            get { return _asctr; }
            set { _asctr = value; }
        }

        /// <summary>
        /// 主站随机数（16字节）
        /// </summary>
        public string RandonHost;//主站随机数
        /// <summary>
        /// 会话协商数据，建立应用连接中的密文1
        /// </summary>
        public string SessionInit;
        /// <summary>
        /// 协商数据签名（4字节），建立应用连接中的客户机签名1
        /// </summary>
        public string Sign;

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="keyVer">密钥版本</param>
        /// <param name="comformance">协商参数信息</param>
        /// <param name="connectType">加密类型</param>
        public void Connect(string keyVer, Consult consult)
        {
            string apdu = "0203";
            apdu += consult.ProtocolVer;
            apdu += consult.ProtocolConformance;
            apdu += consult.FunctionConformance;
            apdu += consult.TxMaxSize;
            apdu += consult.RxMaxSize;
            apdu += consult.MaxSizeFrmNum;
            apdu += consult.MaxApduSize;
            apdu += consult.OverTime;
            apdu += consult.SecurityRule;

            Esam698Service.Esam698ServiceClient ESAMproxy = new Esam698Service.Esam698ServiceClient();

            int iKeyState = Get_KeyState(keyVer);
            string cDiv = Get_Div(iKeyState);
            string cASCTR = _asctr.ToString("X8");

            Esam698Service.InitSessionState initSessionState;
            initSessionState = ESAMproxy.Obj_Meter_Formal_InitSession(iKeyState, cDiv, cASCTR, "01");

            RandonHost = initSessionState.OutRandHost;
            SessionInit = initSessionState.OutSessionInit;
            Sign = initSessionState.OutSign;

            apdu += (SessionInit.Length / 2).ToString("X2");
            apdu += SessionInit;
            apdu += (Sign.Length / 2).ToString("X2");
            apdu += Sign;
            apdu += "00";//没有时间标签

            string txString = MakeSendFrm("43", apdu);
            string ret = string.Empty;
            string res = SendAndRec(txString);
            if (res != null && res.Length > 0)
            {
                Frame698 frm = new Frame698(res);
                if (frm.IsValidFrame)
                {
                    ret = frm.ApduStr;
                }
            }
        }

        /// <summary>
        /// 获取分散因子（8字节）
        /// </summary>
        /// <param name="iKeyState">对称密钥状态（0：出产密钥；1：正式密钥）</param>
        /// <returns>分散因子</returns>
        private string Get_Div(int iKeyState)
        {
            if (iKeyState == 0)
            {
                return _serialNo.ToHexString();
            }
            else if (iKeyState == 1)
            {
                return _meterNo.ToHexString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取KeyState的值
        /// 0 出厂密钥
        /// 1 正式密钥
        /// </summary>
        /// <param name="keyVer">对称密钥版本</param>
        /// <returns>0/1</returns>
        private static int Get_KeyState(string keyVer)
        {
            if (keyVer.Substring(0, 9).ToUpper() == "7FFFFFFFF")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
        private enum ApplicationServerType : byte
        {
            CONNECT_Request = 0x02,
            RELEASE_Request = 0x03,
            GET_Request     = 0x05,
            SET_Request     = 0x06,
            ACTION_Request  = 0x07,
            REPORT_Response = 0x08,
            PROXY_Request   = 0x09,
            ERROR_Response  = 0x6E,
            SECURITY_Request= 0x10
        }


        private string MakeSendFrm(string ctl, string apdu)
        {
            string frm = string.Empty;
            int cnt = 0;
            frm += "68";
            frm += "L1L2";//2字节长度
            cnt += 2;
            frm += ctl;//1字节
            cnt++;
            frm += "AF";//地址标识AF
            cnt++;
            frm += Addr;
            cnt += Addr.Length / 2;
            frm += "10";//客户机地址
            cnt++;

            string hStr = frm.Substring(2);
            frm += "HCS";//2字节帧头校验
            cnt += 2;
            frm += apdu;
            cnt += apdu.Length / 2;
            frm += "FCS";
            cnt += 2;
            frm += "16";

            frm = frm.Replace("L1L2", (cnt & 0x00ff).ToString("X2") + (cnt >> 8).ToString("X2"));
            hStr = hStr.Replace("L1L2", (cnt & 0x00ff).ToString("X2") + (cnt >> 8).ToString("X2"));
            frm = frm.Replace("AF", (Addr.Length / 2 - 1).ToString("X2"));
            hStr = hStr.Replace("AF", (Addr.Length / 2 - 1).ToString("X2"));
            frm = frm.Replace("HCS", GetCS(hStr));
            string fStr = frm.Substring(2, 2*(cnt - 2));
            frm = frm.Replace("FCS", GetCS(fStr));

            return frm;
        }

        private string MakeApduFrm()
        {
            string frm = string.Empty;
            return frm;
        }

        /// <summary>
        /// 判断是否读完
        /// </summary>
        /// <param name="frm">输入帧</param>
        /// <returns></returns>
        protected override bool ReceiveFinishJudge(byte[] frm)
        {
            Frame698 frame698 = new Frame698(frm);
            return frame698.IsValidFrame;
        }

        protected override void AnayzeFrm(string frm)
        {
            Frame698 frame698 = new Frame698(frm);
        }
    }

    public class Consult
    {
        public string ProtocolVer;
        public string ProtocolConformance;
        public string FunctionConformance;
        public string TxMaxSize;
        public string RxMaxSize;
        public string MaxSizeFrmNum;
        public string MaxApduSize;
        public string OverTime;
        public string SecurityRule;
    }


    
}
