using System;
using Common;

namespace Protocol.Core
{
    public class Frame698
    {
        private const int MAXFRAMELEN    = 512;
        private const int MINFRAMELEN    = 17;
        private const int OADLEN         = 4;
        private const int OFFSET_START68 = 0;
        private const int OFFSET_LEN     = 1;
        private const int OFFSET_CTRLNUM = 3;
        private const int OFFSET_SA      = 4;

        #region 698帧定义
        private string strFrame = null;
        private byte[] byteFrame;
        /// <summary>
        /// 帧长度
        /// </summary>
        private int frameLen = MAXFRAMELEN;
        private int first68Index = 0;
        /// <summary>
        /// 服务器地址
        /// </summary>
        private byte[] SA;
        private byte saLen;
        /// <summary>
        /// 客户机地址
        /// </summary>
        private byte CA;
        /// <summary>
        /// 控制码
        /// </summary>
        private byte C;
        /// <summary>
        /// 数据长度
        /// </summary>
        private ushort L;
        /// <summary>
        /// 头校验和
        /// </summary>
        private byte[] HCS = new byte[2];
        /// <summary>
        /// 帧校验
        /// </summary>
        private byte[] FCS = new byte[2];
        /// <summary>
        /// APDU
        /// </summary>
        private byte[] APDU;
        private byte APDUlen;
        #endregion
        private bool isValidFrame = true;
        #region 属性
        public bool IsValidFrame
        {
            get { return isValidFrame; }
        }
        public string FrameStart68
        {
            get { return byteFrame[OFFSET_START68].ToString("X2"); }
        }
        public string FrameEnd16
        {
            get { return byteFrame[frameLen - 1].ToString("X2"); }
        }
        public string LengthStr
        {
            get { return Length.ToString().PadLeft(4,'0'); }
        }
        public string LengthFrame
        {
            get
            {
                string tmp = "";
                tmp += (L & 0x00ff).ToString("X2") + " ";
                tmp += (L & 0xff00).ToString("X2");
                return tmp;
            }
        }
        /// <summary>
        /// 长度
        /// (除起始和结束字符外的所有帧字节数)
        /// </summary>
        public ushort Length
        {
            get { return L; }
        }
        /// <summary>
        /// 长度
        /// 高字节在前，低字节在后
        /// </summary>
        public byte[] Length_byte
        {
            get
            {
                byte[] tmp = new byte[2];
                tmp[0] = (byte)((L & 0xff00) >> 8);
                tmp[1] = (byte)( L & 0x00ff);
                return tmp;
            }
        }
        /// <summary>
        /// 控制码
        /// </summary>
        public byte Ctrl
        {
            get { return C; }
        }
        public string CtrlStr
        {
            get { return C.ToString("X2"); }
        }
        /// <summary>
        /// 传输方向位DIR和启动标志位PRM组合
        /// </summary>
        public byte DIR_PRM
        {
            get { return (byte)(C & 0xc0); }
        }
        /// <summary>
        /// 分帧标志
        /// </summary>
        public byte DivFrameFlag
        {
            get { return (byte)(C & 0x20); }
        }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode
        {
            get { return (byte)(C & 0x07); }
        }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public byte[] Sa
        {
            get { return SA; }
        }
        /// <summary>
        /// 服务器地址string形式
        /// （带地址类型，未反转）
        /// </summary>
        public string SaFrame
        {
            get { return SA.ToHexString(); }
        }

        /// <summary>
        /// 服务器地址string形式
        /// </summary>
        public string SaStr
        {
            get{ return SA.ToHexString().Substring(2).ReverseStr(); }
        }
        /// <summary>
        /// 服务器地址类型
        /// </summary>
        public byte SaType
        {
            get { return (byte)(SA[0] & 0xc0); }
        }
        /// <summary>
        /// 服务器逻辑地址
        /// </summary>
        public byte SaLogicAddr
        {
            get { return (byte)(SA[0] & 0x30); }
        }
        /// <summary>
        /// 地址长度
        /// </summary>
        public byte SaLength
        {
            get { return (byte)(SA[0] & 0x0f); }
        }
        /// <summary>
        /// 客户机地址
        /// </summary>
        public byte Ca
        {
            get { return CA; }
        }
        public string CaStr
        {
            get { return CA.ToString("X2"); }
        }
        /// <summary>
        /// 帧头校验和
        /// (高字节在前，低字节在后)
        /// </summary>
        public byte[] Hcs
        {
            get { return HCS; }
        }
        public string HcsFrame
        {
            get
            {
                string tmp = "";
                tmp += HCS[1].ToString("X2") + " ";
                tmp += HCS[0].ToString("X2");
                return tmp;
            }
        }
        /// <summary>
        /// 帧校验和
        /// (高字节在前，低字节在后)
        /// </summary>
        public byte[] Fcs
        {
            get { return FCS; }
        }
        public string FcsFrame
        {
            get
            {
                string tmp = "";
                tmp += FCS[1].ToString("X2") + " ";
                tmp += FCS[0].ToString("X2");
                return tmp;
            }
        }
        /// <summary>
        /// APDU链路用户数据
        /// </summary>
        public byte[] Apdu
        {
            get { return APDU; }
        }
        public string ApduFrame
        {
            get//{APDU.ToHexString();}
            {
                string tmp = "";
                for(int i = 0; i < APDU.Length; i++)
                {
                    tmp += APDU[i].ToString("X2") + " ";
                }
                return tmp;
            }
        }
        public string ApduStr
        {
            get { return APDU.ToHexString(); }
        }
        #endregion
        public Frame698(byte[] frame)
        {
            byteFrame = frame;
            strFrame = frame.ToHexString();
            Analyze698Frame();
        }
        public Frame698(string frame)
        {
            strFrame = frame.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            byteFrame = Transfer.StrToByte(frame);
            Analyze698Frame();
        }
        private void Analyze698Frame()
        {
            if (!Check698Frame()) return;
            AnalyzeAPDUReagion();
        }
        private void AnalyzeAPDUReagion()
        {

        }
        private bool Check698Frame()
        {
            isValidFrame = false;
            if (!FindFirst68Index()) return false;

            //检查长度
            ushort len = (ushort)((byteFrame[first68Index + 2] << 8) + byteFrame[first68Index + 1]);
            if ((byteFrame.Length - first68Index - 2) != len) return false;

            //检查头校验
            int tmpSaLen = (byteFrame[first68Index + OFFSET_SA] & 0x0f) + 1;
            int hcsOffsetPos = first68Index + OFFSET_SA + tmpSaLen + 2;
            ushort hcs = (ushort)(byteFrame[hcsOffsetPos + 1] << 8 + byteFrame[hcsOffsetPos]);            
            string headFrm = "";
            for(int i = first68Index; i < hcsOffsetPos; i++)
            {
                headFrm += byteFrame[i].ToHexString();
            }
            //if (hcs != Protocol698.pppfcs16(headFrm)) return false;        //暂未开放校验

            //检查帧校验
            ushort fcs = (ushort)(byteFrame[byteFrame.Length - 2] << 8 + byteFrame[byteFrame.Length - 3]);
            string fFrm = "";
            for (int i = first68Index; i < byteFrame.Length - 3; i++)
            {
                fFrm += byteFrame[i].ToHexString();
            }
            //if (fcs != Protocol698.pppfcs16(fFrm)) return false;        //暂未开放校验

            //检查结束字符
            if (byteFrame[byteFrame.Length - 1] != 0x16) return false;

            //检查结束后进行全局变量赋值
            isValidFrame = true;
            frameLen = byteFrame.Length - first68Index;
            byte[] tmpFrm = new byte[frameLen];
            for (int i = 0; i < frameLen; i++)
            {
                tmpFrm[i] = byteFrame[first68Index + i];
            }
            byteFrame = tmpFrm;
            L = len;
            C = byteFrame[OFFSET_CTRLNUM];
            saLen = (byte)tmpSaLen;
            SA = new byte[saLen + 1];
            SA[0] = byteFrame[OFFSET_SA];
            for(int i = 0; i < saLen; i++)
            {
                SA[i+1] = byteFrame[OFFSET_SA + saLen - i];
            }
            CA = byteFrame[OFFSET_SA + saLen + 1];
            HCS[0] = byteFrame[OFFSET_SA + saLen + 3];
            HCS[1] = byteFrame[OFFSET_SA + saLen + 2];
            int offsetAPDU = OFFSET_SA + saLen + 4;
            APDUlen = (byte)(frameLen - offsetAPDU - 3);
            APDU = new byte[APDUlen];
            for(int i = 0; i < APDUlen; i++)
            {
                APDU[i] = byteFrame[offsetAPDU + i];
            }
            FCS[0] = byteFrame[frameLen - 2];
            FCS[1] = byteFrame[frameLen - 3];
            return true;
        }
        private bool FindFirst68Index()
        {
            if (byteFrame.Length < MINFRAMELEN) return false;
            if (byteFrame[0] == 0xfe || byteFrame[0] == 0x68)
            {
                first68Index = Array.IndexOf<byte>(byteFrame, 0x68);
                return true;
            }
            else
                return false;
        }


    }
    class APDU
    {
        public const byte LINK_REQUEST  = 1;
        public const byte LINK_RESPONSE = 129;
        public const byte CONNECT_REQUEST = 2;
        public const byte CONNECT_RESPONSE = 130;
        public const byte RELEASE_REQUEST = 3;
        public const byte RELEASE_RESPONSE = 131;
        public const byte RELEASE_NOTIFICATION = 132;
        public const byte GET_REQUEST = 5;
        public const byte GET_RESPONSE = 133;
        public const byte SET_REQUEST = 6;
        public const byte SET_RESPONSE = 134;
        public const byte ACTION_REQUEST = 7;
        public const byte ACTION_RESPONSE = 135;
        public const byte REPORT_RESPONSE = 8;
        public const byte REPORT_NOTIFICATION = 136;
        public const byte PROXY_REQUEST = 9;
        public const byte PROXY_RESPONSE = 137;
        public const byte ERROR_1 = 110;
        public const byte ERROR_2 = 238;
        public const byte SECURITY_REQUEST = 16;
        public const byte SECURITY_RESPONSE = 144;

        private byte _ctrl1;//操作类型1级
        private byte _ctrl2;//操作类型2级
        private byte _piid;
        private byte[,] _oad;
        private byte _dataType;
        private byte _lenOrNum;
        private bool _hasData;
        private byte[,] _data;
        private byte[,] _dar;
        private byte _followReport;
        private byte _timeTag;
        private bool _isSecurity;
        private bool _isPlain;
        private byte _plainOrCipherLen;
        private byte[] _plainOrCipher;

        private bool _isValid;

        private readonly byte[] UnknowNumDataType = new byte[]{1, 2, 4, 9, 10, 85};//未知长度的数据类型

        public APDU(byte[] apdu)
        {
            AnalyzeApdu(apdu);
        }

        private void AnalyzeApdu(byte[] apdu)
        {
            int offset = 0;
            if (apdu[offset] == SECURITY_REQUEST || apdu[offset] == SECURITY_RESPONSE)
            {
                _isSecurity = true;
                offset++;
                _isPlain = (apdu[offset] == 0) ? true : false;
                offset++;
                _plainOrCipherLen = apdu[offset];
                offset++;
                _plainOrCipher = new byte[_plainOrCipherLen];
                Array.Copy(apdu, offset, _plainOrCipher, 0, _plainOrCipherLen);
                offset += _plainOrCipherLen;

                if (_isPlain)//如果是明文
                {
                    AnalyzeApdu(_plainOrCipher);
                }
                else//如果是密文
                {
                }
            }
            _ctrl1 = apdu[offset];
            offset++;
            switch (_ctrl1)
            {
                case GET_REQUEST:
                    break;
                default:
                    break;
            }
        }

        private void AnalyzeGET(byte[] frm)
        {
            int offset = 0;
            _ctrl2 = frm[offset];
            offset++;
            _piid = frm[offset];
            offset++;
            if (_ctrl1 == GET_REQUEST)
            {
                AnalyzeGetRequest(frm, ref offset);
            }
            else if (_ctrl1 == GET_RESPONSE)
            {
                AnalyzeGetResponse(frm, ref offset);
            }
        }

        private void AnalyzeGetRequest(byte[] frm, ref int offset)
        {
            switch (_ctrl2)
            {
                case 1://get request normal
                    _oad = new byte[1, 4];
                    for (int i = 0; i < 4; i++)
                    {
                        _oad[0, i] = frm[offset++];
                    }
                    break;
                case 2://get request normal list
                    byte num = frm[offset];
                    offset++;
                    _oad = new byte[num, 4];
                    for (int i = 0; i < num; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            _oad[i, j] = frm[offset++];
                        }
                    }
                    break;
                case 3://get request record
                case 4://get request record list
                case 5://get request next
                case 6://get request MD5
                    break;
                default:
                    break;
            }
            _timeTag = frm[offset];
            offset++;
        }

        private void AnalyzeGetResponse(byte[] frm, ref int offset)
        {
            switch (_ctrl2)
            {
                case 1://get response normal
                    _oad = new byte[1, 4];
                    for (int i = 0; i < 4; i++)
                    {
                        _oad[0, i] = frm[offset++];
                    }
                    _hasData = (frm[offset] == 1) ? true : false;
                    offset++;
                    if (_hasData)
                    {
                        
                    }
                    else
                    {
                        _dar[0,0] = frm[offset];
                        offset++;
                    }
                    break;
                case 2://get response normal list
                    byte num = frm[offset];
                    offset++;
                    _oad = new byte[num, 4];
                    for (int i = 0; i < num; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            _oad[i, j] = frm[offset++];
                        }
                        _hasData = (frm[offset] == 1) ? true : false;
                        offset++;
                        if (_hasData)
                        {

                        }
                        else
                        {
                            _dar[i,0] = frm[offset];
                            offset++;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        //private int num;
        private void GetDataRegion(byte[] frm, ref int offset)
        {
            byte arrayNum = 0;
            byte num = 0;
            _dataType = frm[offset];
            offset++;
            if (_dataType == 1 || _dataType == 2)//array, structure
            {
                num = frm[offset];
                arrayNum = num;
                offset++;
                for (; num != 0; num--)
                {
                    GetDataRegion(frm, ref offset);
                }
            }

            if (Array.IndexOf(UnknowNumDataType, _dataType) == -1)//没有个数或长度
            {
                int len = GetLenByType(_dataType);
                _data = new byte[arrayNum, len];
                for (int i = 0; i < len; i++)
                {
                    _data[arrayNum - num, i] = frm[offset++];
                }
            }
            else
            {
                byte len = frm[offset];
                offset++;
                _data = new byte[1, len];
                for (int i = 0; i < len; i++)
                {
                    _data[0, i] = frm[offset++];
                }
            }
        }

        private int GetLenByType(byte type)
        {
            int len = 0;
            switch (type)
            {
                case 0: 
                    len = 0; break;
                case 3:
                case 15:
                case 17:
                case 22:
                    len = 1; break;
                case 16:
                case 18:
                case 80:
                    len = 2; break; 
                case 27:
                    len = 3; break;
                case 5:
                case 6:
                case 23:
                case 81:
                case 83:
                    len = 4; break;
                case 26:
                    len = 5; break;
                case 28:
                    len = 7; break;
                case 20:
                case 21:
                case 24:
                    len = 8; break;
                case 25:
                    len = 10; break;
                default:
                    break;
            }
            return len;
        }
    }
    class MaxDemand
    {
        private byte[] OI = new byte[2];
        
        private byte[] OAD = new byte[4];
        public byte[] GetOAD
        {
            get { return OAD; }
        }

        public byte[] No1_LogicName
        {
            get { return OI; }
        }
        //public byte[] No2_

        public MaxDemand(byte[] oi)
        {
            OI = oi;
        }
        //public MaxDemand(byte[] oad)
        //{
        //    OAD = oad;
        //}
    }
}
