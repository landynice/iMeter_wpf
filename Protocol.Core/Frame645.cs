using System;
using Common;

namespace Protocol.Core
{
    public class Frame645
    {
        private const int MAXFRAMELEN      = 200;
        private const int MINFRAMELEN      = 12;
        private const int OFFSET_START68   = 0;
        private const int OFFSET_METERADDR = 1;
        private const int OFFSET_SECOND68  = 7;
        private const int OFFSET_CTRLNUM   = 8;
        private const int OFFSET_LEN       = 9;
        private const int DATAIDLEN        = 4;
        private const int PSWLEN           = 4;
        private const int OPRLEN           = 4;
        private const int ADDRLEN          = 6;

        #region 645帧定义
        byte[] ByteFrame;
        /// <summary>
        /// 帧长度
        /// </summary>
        public int FrameLen = MAXFRAMELEN;
        public int First68Index = 0;
        public int DataLen;
        #endregion

        /// <summary>
        /// 控制码
        /// </summary>
        private enum Func
        {
            ReadData              = 0x11,
            ReadDataHasFollow     = 0xB1,
            ReadDataSucc          = 0x91,
            ReadDataFail          = 0xD1,

            ReadFollowData        = 0x12,
            ReadFollowDataHasFollow = 0xB2,
            ReadFollowDataSucc    = 0x92,
            ReadFollowDataFail    = 0xD2,

            WriteData             = 0x14,
            WriteDataSucc         = 0x94,
            WriteDataFail         = 0xD4,

            ReadAddr              = 0x13,
            ReadAddrSucc          = 0x93,

            WriteAddr             = 0x15,
            WriteAddrSucc         = 0x95,

            BroadcastSetTime      = 0x08,

            Freeze                = 0x16,
            FreezeSucc            = 0x96,
            FreezeFail            = 0xD6,

            ChangeBaudrate        = 0x17,
            ChangeBaudrateSucc    = 0x97,
            ChangeBaudrateFail    = 0xD7,

            ChangePassword        = 0x18,
            ChangePasswordSucc    = 0x98,
            ChangePasswordFail    = 0xD8,

            MaxDemandClear        = 0x19,
            MaxDemandClearSucc    = 0x99,
            MaxDemandClearFail    = 0xD9,

            MeterClear            = 0x1A,
            MeterClearSucc        = 0x9A,
            MeterClearFail        = 0xDA,

            EventClear            = 0x1B,
            EventClearSucc        = 0x9B,
            EventClearFail        = 0xDB,

            CtrlOrder             = 0x1C,
            CtrlOrderSucc         = 0x9C,
            CtrlOrderFail         = 0xDC,

            MultifunctionCtrl     = 0x1D,
            MultifunctionCtrlSucc = 0x9D,
            MultifunctionCtrlFail = 0xDD,

            SecurityCert          = 0x03,
            SecurityCertSucc      = 0x83,
            SecurityCertFail      = 0xC3
        }
        private Func func645;
        private enum ErrStyle : byte
        {
            OtherErr          = 0x01,
            NoRequestData     = 0x02,
            PswErrOrNoPwr     = 0x04,
            CantChgeBaud      = 0x08,
            YearTmZoneNumOver = 0x10,
            DayTmSegNumOver   = 0x20,
            TriffNumOver      = 0x40,
            Hold              = 0x80
        }
        private enum SerrStyle : ushort
        {
            OtherErr       = 0x0001,
            RepeatRechage  = 0x0002,
            ESAMCertFail   = 0x0004,
            IdentityFail   = 0x0008,
            ClientNumNoFit = 0x0010,
            RechangeNumErr = 0x0020,
            BuyElecOver    = 0x0040,
            Hold1          = 0x0080,
            Hold2          = 0x0100,
            Hold3          = 0x0200,
            Hold4          = 0x0400,
            Hold5          = 0x0800,
            Hold6          = 0x1000,
            Hold7          = 0x2000,
            Hold8          = 0x4000,
            Hold9          = 0x8000
        }
        #region 属性
        /// <summary>
        /// 属性：是否为有效645格式的报文
        /// </summary>
        public bool IsValidFrame { get; private set; }
        /// <summary>
        /// 电表地址
        /// </summary>
        public string MeterAddress { get { return hexMeterAddress.ToHexString(); } }
        /// <summary>
        /// 电表地址
        /// </summary>
        public byte[] hexMeterAddress { get { return _hexMeterAddress; } }
        byte[] _hexMeterAddress = new byte[6];
        /// <summary>
        /// 控制码
        /// </summary>
        public string CtrolNum { get{return hexCtrolNum.ToHexString();} }
        /// <summary>
        /// 控制码
        /// </summary>
        public byte hexCtrolNum { get; private set; }
        /// <summary>
        /// 数据域长度
        /// </summary>
        public string DataLength { get{return hexDataLength.ToHexString();} }
        /// <summary>
        /// 数据域长度
        /// </summary>
        public byte hexDataLength { get; private set; }
        /// <summary>
        /// 数据标识符
        /// </summary>
        public string DataId { get{ return hexDataId.ToHexString();} }
        /// <summary>
        /// 数据标识符
        /// </summary>
        public byte[] hexDataId { get { return _hexDataId; } }
        byte[] _hexDataId = new byte[4];
        /// <summary>
        /// 电表密码
        /// </summary>
        public string Password { get{return hexPassword.ToHexString();} }
        /// <summary>
        /// 电表密码
        /// </summary>
        public byte[] hexPassword { get { return _hexPassword; } }
        byte[] _hexPassword = new byte[4];
        /// <summary>
        /// 操作者代码
        /// </summary>
        public string OperateCode { get{return hexOperateCode.ToHexString();} }
        /// <summary>
        /// 操作者代码
        /// </summary>
        public byte[] hexOperateCode { get { return _hexOperateCode; } }
        byte[] _hexOperateCode = new byte[4];
        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get{return hexData.ToHexString();} }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] hexData { get; private set; }
        /// <summary>
        /// 数据域的全部数据
        /// </summary>
        public string OriginalData { get { return hexOriginalData.ToHexString(); } }
        /// <summary>
        /// 数据域的全部数据
        /// </summary>
        public byte[] hexOriginalData
        {
            get
            {
                byte[] hexData = new byte[hexDataLength];
                if (hexDataLength == 0) return null;
                for(int i = 0; i < hexDataLength; i++)
                {
                    hexData[i] = Counter.ByteSub33H(ByteFrame[OFFSET_LEN + 1 + i]);
                }
                return hexData;
            }
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Err { get {return hexErr.ToHexString();} }
        /// <summary>
        /// 错误代码
        /// </summary>
        public byte hexErr { get; private set; }
        /// <summary>
        /// 安全信息错误代码
        /// </summary>
        public string Serr { get { return hexSerr.ToHexString(); } }
        /// <summary>
        /// 安全信息错误代码
        /// </summary>
        public byte[] hexSerr { get { return _hexSerr; } }
        byte[] _hexSerr = new byte[2];
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo { get; private set; } 
        /// <summary>
        /// 分帧序号
        /// </summary>
        public string Seq { get { return hexSeq.ToHexString(); } }
        /// <summary>
        /// 分帧序号
        /// </summary>
        public byte hexSeq { get; private set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get{return hexCheckSum.ToHexString();} }
        /// <summary>
        /// 校验和
        /// </summary>
        public byte hexCheckSum { get; private set; }
        /// <summary>
        /// 功能
        /// </summary>
        public string Function { get{ return func645.ToString();} }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strFrame645">输入整条645报文，可以带空格、其他字符</param>
        public Frame645(string strFrame645)
        {
            IniteParams();
            if (strFrame645 == null)
            {
                IsValidFrame = false;
                return;
            }
            ByteFrame = Transfer.StrToByte(strFrame645);
            Analyze645Frame();
        }
        public Frame645(byte[] frame645)
        {
            IniteParams();
            if (frame645 == null)
            {
                IsValidFrame = false;
                return;
            }
            ByteFrame = frame645;
            Analyze645Frame();
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        private void IniteParams()
        {
            ErrInfo = "正常应答";
            IsValidFrame = true;
        }
        /// <summary>
        /// 分析645帧
        /// </summary>
        private void Analyze645Frame()
        {
            if (!Check645Frame()) return;
            for(int i = 0; i < 6; i++)
            {
                _hexMeterAddress[i] = ByteFrame[6 - i];
            }
            hexCtrolNum = ByteFrame[OFFSET_CTRLNUM];
            hexDataLength = ByteFrame[OFFSET_LEN];
            hexCheckSum = ByteFrame[FrameLen - 2];
            AnalyzeFrameDataReagion();
        }
        /// <summary>
        /// 分析数据域
        /// </summary>
        private void AnalyzeFrameDataReagion()
        {
            switch(hexCtrolNum)
            {
                case 0x11://读数据
                    func645 = Func.ReadData;
                    GetDataId();
                    break;
                case 0x91://正常应答
                    func645 = Func.ReadDataSucc;
                    GetDataId();
                    DataLen = hexDataLength - DATAIDLEN;
                    GetData(DataLen);
                    break;
                case 0xB1://有后续帧
                    func645 = Func.ReadDataHasFollow;
                    GetDataId();
                    DataLen = hexDataLength - DATAIDLEN;
                    GetData(DataLen);
                    break;
                case 0xD1://异常应答
                    func645 = Func.ReadDataFail;
                    GetErr();
                    break;
                case 0x12://读后续数据
                    func645 = Func.ReadFollowData;
                    GetDataId();
                    break;
                case 0x92://无后续数据帧
                    func645 = Func.ReadFollowDataSucc;
                    GetDataId();
                    DataLen = hexDataLength - DATAIDLEN - 1;
                    GetData(DataLen);
                    break;
                case 0xB2://有后续数据
                    func645 = Func.ReadFollowDataHasFollow;
                    GetDataId();
                    DataLen = hexDataLength - DATAIDLEN - 1;
                    GetData(DataLen);
                    break;
                case 0xD2://异常应答
                    func645 = Func.ReadFollowDataFail;
                    GetErr();
                    break;
                case 0x14://写数据
                    func645 = Func.WriteData;
                    GetDataId();
                    GetPassword();
                    GetOperateCode();
                    DataLen = hexDataLength - DATAIDLEN - PSWLEN - OPRLEN;
                    GetData(DataLen);
                    break;
                case 0x94://正常应答
                    func645 = Func.WriteDataSucc;
                    break;
                case 0xD4://异常应答
                    func645 = Func.WriteDataFail;
                    GetErr();
                    break;
                case 0x13://读通信地址
                    func645 = Func.ReadAddr;
                    break;
                case 0x93://正常应答
                    func645 = Func.ReadAddrSucc;
                    GetAddr();
                    break;
                case 0x15://写通信地址
                    func645 = Func.WriteAddr;
                    GetAddr();
                    break;
                case 0x95://正常应答
                    func645 = Func.WriteAddrSucc;
                    break;
                case 0x08://广播校时
                    func645 = Func.BroadcastSetTime;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0x16://冻结命令
                    func645 = Func.Freeze;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0x96://正常应答
                    func645 = Func.FreezeSucc;
                    break;
                case 0xD6://异常应答
                    func645 = Func.FreezeFail;
                    GetErr();
                    break;
                case 0x17://更改通信速率
                    func645 = Func.ChangeBaudrate;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0x97://正常应答
                    func645 = Func.ChangeBaudrateSucc;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0xD7://异常应答
                    func645 = Func.ChangeBaudrateFail;
                    GetErr();
                    break;
                case 0x18://修改密码
                    func645 = Func.ChangePassword;
                    GetDataId();
                    GetPassword();
                    DataLen = hexDataLength - DATAIDLEN - PSWLEN;
                    GetData(DataLen);
                    break;
                case 0x98://正常应答
                    func645 = Func.ChangePasswordSucc;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0xD8://异常应答
                    func645 = Func.ChangePasswordFail;
                    GetErr();
                    break;
                case 0x19://最大需量清零
                    func645 = Func.MaxDemandClear;
                    GetPassword(FrameLen - 3 - OPRLEN);
                    GetOperateCode(FrameLen - 3);
                    break;
                case 0x99://正常应答
                    func645 = Func.MaxDemandClearSucc;
                    break;
                case 0xD9://异常应答
                    func645 = Func.MaxDemandClearFail;
                    GetErr();
                    break;
                case 0x1A://电表清零
                    func645 = Func.MeterClear;
                    GetPassword(FrameLen - 3 - OPRLEN);
                    GetOperateCode(FrameLen - 3);
                    break;
                case 0x9A://正常应答
                    func645 = Func.MeterClearSucc;
                    break;
                case 0xDA://异常应答
                    func645 = Func.MeterClearFail;
                    GetErr();
                    break;
                case 0x1B://事件清零
                    func645 = Func.EventClear;
                    GetPassword(13);
                    GetOperateCode(17);
                    DataLen = hexDataLength - PSWLEN - OPRLEN;
                    GetData(DataLen);
                    break;
                case 0x9B://正常应答
                    func645 = Func.EventClearSucc;
                    break;
                case 0xDB://异常应答
                    func645 = Func.EventClearFail;
                    GetErr();
                    break;
                case 0x1C://控制命令
                    func645 = Func.CtrlOrder;
                    GetPassword(13);
                    GetOperateCode(17);
                    DataLen = hexDataLength - PSWLEN - OPRLEN;
                    GetData(DataLen);
                    break;
                case 0x9C://正常应答
                    func645 = Func.CtrlOrderSucc;
                    break;
                case 0xDC://异常应答
                    func645 = Func.CtrlOrderFail;
                    GetErr();
                    break;
                case 0x1D://多功能端子输出控制命令
                    func645 = Func.MultifunctionCtrl;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0x9D://正常应答
                    func645 = Func.MultifunctionCtrlSucc;
                    DataLen = hexDataLength;
                    GetData(DataLen);
                    break;
                case 0xDD://异常应答
                    func645 = Func.MultifunctionCtrlFail;
                    GetErr();
                    break;
                case 0x03://安全认证命令
                    func645 = Func.SecurityCert;
                    GetDataId();
                    GetOperateCode(17);
                    DataLen = hexDataLength - DATAIDLEN - OPRLEN;
                    GetData(DataLen);
                    break;
                case 0x83://正常应答
                    func645 = Func.SecurityCertSucc;
                    GetDataId();
                    DataLen = hexDataLength - DATAIDLEN;
                    GetData(DataLen);
                    break;
                case 0xC3://异常应答
                    func645 = Func.SecurityCertFail;
                    GetSerr();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取数据ID
        /// </summary>
        /// <param name="dataIdStartAddr"></param>
        private void GetDataId(int dataIdStartAddr = OFFSET_LEN + DATAIDLEN)
        {
            for(int i = 0; i < 4; i++)
            {
                _hexDataId[i] = Counter.ByteSub33H(ByteFrame[dataIdStartAddr - i]);
            }
        }
        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="passwordStartAddr"></param>
        private void GetPassword(int passwordStartAddr = OFFSET_LEN + DATAIDLEN + PSWLEN)
        {
            for(int i = 0; i < 4; i++)
            {
                _hexPassword[i] = Counter.ByteSub33H(ByteFrame[passwordStartAddr - i]);
            }
        }
        /// <summary>
        /// 获取操作者代码
        /// </summary>
        /// <param name="operateCodeStartAddr"></param>
        private void GetOperateCode(
            int operateCodeStartAddr = OFFSET_LEN + DATAIDLEN + PSWLEN + OPRLEN)
        {
            for (int i = 0; i < 4; i++)
            {
                _hexOperateCode[i] = Counter.ByteSub33H(ByteFrame[operateCodeStartAddr - i]);
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="dataLength"></param>
        private void GetData(int dataLength)
        {
            hexData = new byte[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                hexData[i] = Counter.ByteSub33H(ByteFrame[FrameLen - 3 - i]);
            }
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        private void GetErr()
        {
            hexErr = Counter.ByteSub33H(ByteFrame[FrameLen - 3]);
            switch (hexErr)
            {
                case (byte)ErrStyle.OtherErr:          ErrInfo = "其他错误";         break;
                case (byte)ErrStyle.NoRequestData:     ErrInfo = "无请求数据";       break;
                case (byte)ErrStyle.PswErrOrNoPwr:     ErrInfo = "密码错/未授权";    break;
                case (byte)ErrStyle.CantChgeBaud :     ErrInfo = "通信速率不能更改"; break;
                case (byte)ErrStyle.YearTmZoneNumOver: ErrInfo = "年时区数超";       break;
                case (byte)ErrStyle.DayTmSegNumOver:   ErrInfo = "日时段数超";       break;
                case (byte)ErrStyle.TriffNumOver:      ErrInfo = "费率数超";         break;
                case (byte)ErrStyle.Hold:              ErrInfo = "保留";             break;
                default:                               ErrInfo = "ERR_wrong";        break;
            }
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        private void GetSerr()
        {
            UInt16 serr = 0;
            for(int i = 0; i < 2; i++)
            {
                _hexSerr[i] = Counter.ByteSub33H(ByteFrame[FrameLen - 3 - i]);
            }
            serr = (ushort)(((ushort)_hexSerr[0] << 8) + _hexSerr[1]);
            switch (serr)
            {
                case (ushort)SerrStyle.OtherErr:       ErrInfo = "其他错误";       break;
                case (ushort)SerrStyle.RepeatRechage:  ErrInfo = "重复充值";       break;
                case (ushort)SerrStyle.ESAMCertFail:   ErrInfo = "ESAM验证失败";   break;
                case (ushort)SerrStyle.IdentityFail:   ErrInfo = "身份认证失败";   break;
                case (ushort)SerrStyle.ClientNumNoFit: ErrInfo = "客户编号不匹配"; break;
                case (ushort)SerrStyle.RechangeNumErr: ErrInfo = "充值次数错误";   break;
                case (ushort)SerrStyle.BuyElecOver:    ErrInfo = "购电超囤积";     break;
                case (ushort)SerrStyle.Hold1:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold2:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold3:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold4:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold5:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold6:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold7:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold8:          ErrInfo = "保留";           break;
                case (ushort)SerrStyle.Hold9:          ErrInfo = "保留";           break;
                default:                               ErrInfo = "SERR_wrong";     break;
            }
        }
        /// <summary>
        /// 获取帧序号
        /// </summary>
        private void GetSeq()
        {
            if (hexCtrolNum == 0x12 || hexCtrolNum == 0x92 || hexCtrolNum == 0xB2)
            {
                hexSeq = Counter.ByteSub33H(ByteFrame[FrameLen - 3]);
            }
            else
                hexSeq = 0;
        }
        /// <summary>
        /// 获取表地址
        /// </summary>
        private void GetAddr()
        {
            if (hexCtrolNum == 0x93 || hexCtrolNum == 0x15)
            {
                for (int i = 0; i < ADDRLEN; i++)
                {
                    _hexMeterAddress[i] = Counter.ByteSub33H(ByteFrame[FrameLen - 3 - i]);
                }
            }
            else
            {
                for (int i = 0; i < ADDRLEN; i++)
                {
                    _hexMeterAddress[i] = ByteFrame[ADDRLEN - i];
                }
            }
        }
        /// <summary>
        /// 检查645帧
        /// </summary>
        /// <returns></returns>
        private bool Check645Frame()
        {
            if (!FindFirst68Index())
            {
                IsValidFrame = false;
                return false;
            }

            FrameLen = ByteFrame.Length - First68Index;
            byte[] tmpFrm = new byte[FrameLen];
            for (int i = 0; i < FrameLen; i++)
            {
                tmpFrm[i] = ByteFrame[First68Index + i];
            }
            ByteFrame = tmpFrm;

            if (ByteFrame[OFFSET_LEN] != (FrameLen - 12) )
            {
                IsValidFrame = false;
                return false;
            }

            byte checkSum = 0;
            for (int i = 0; i < FrameLen - 2; i++)
            {
                checkSum += ByteFrame[i];
            }
            if(ByteFrame[FrameLen - 2] != checkSum)
            {
                IsValidFrame = false;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 查找第一个68的位置
        /// </summary>
        /// <returns>true/false</returns>
        private bool FindFirst68Index()
        {
            if (ByteFrame.Length < MINFRAMELEN) return false;
            if (ByteFrame[0] == 0xfe || ByteFrame[0] == 0x68)
            {
                int tmpIndex = Array.IndexOf<byte>(ByteFrame, 0x68);
                if (ByteFrame[tmpIndex + OFFSET_SECOND68] == 0x68 && ByteFrame[ByteFrame.Length - 1] == 0x16)
                {
                    First68Index = tmpIndex;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

    }
}
