using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Common
{
    public class Functions
    {
        #region 显示代码解释
        /// <summary>
        /// 显示代码解释
        /// </summary>
        public static readonly Dictionary<string, string> DISPLAY = new Dictionary<string, string>
        {
            {"FFFFFFFFFF","全屏显示"},
            {"0000900200","当前剩余金额"},
            {"0002800020","当前电价"},
            {"0002800021","当前费率电价"},
            {"000280000B","当前阶梯电价"},
            {"0002010100","A相电压"},
            {"0002010200","B相电压"},
            {"0002010300","C相电压"},
            {"0002020100","A相电流"},
            {"0002020200","B相电流"},
            {"0002020300","C相电流"},
            {"0002030000","总有功功率"},
            {"0002030100","A相有功功率"},
            {"0002030200","B相有功功率"},
            {"0002030300","C相有功功率"},
            {"0002040000","总无功功率"},
            {"0002040100","A相无功功率"},
            {"0002040200","B相无功功率"},
            {"0002040300","C相无功功率"},
            {"0002060000","总功率因数"},
            {"0002060100","A相功率因数"},
            {"0002060200","B相功率因数"},
            {"0002060300","C相功率因数"},
            {"0002800001","零线电流"},
            {"0002800008","时钟电池电压"},
            {"000280000A","时钟电池使用时间"},
            {"0004000101","当前日期"},
            {"0004000102","当前时间"},
            {"0004000103","最大需量周期"},
            {"0004000104","滑差时间"},
            {"0004000105","校表脉冲宽度"},
            {"0004000401","表地址(国网:高4位；南网:低8位)"},
            {"0104000401","表地址(国网:低8位；南网:高4位)"},
            {"0004000402","表号(国网:高4位；南网:低8位)"},
            {"0104000402","表号(国网:低8位；南网:高4位)"},
            {"000400040E","用户号(国网:高4位；南网:低8位)"},
            {"010400040E","用户号(国网:低8位；南网:高4位)"},
            {"0004000409","有功脉冲常数"},
            {"000400040A","无功脉冲常数"},
            {"0004000703","通讯波特率"},
            {"0004000B01","第1结算日"},
            {"0004000B02","第2结算日"},
            {"0004000B03","第3结算日"},
            {"0004001001","报警金额1"},
            {"0004001002","报警金额2"},
            {"0004001003","透支金额"},
            {"0004050101","当前套费率1"},
            {"0004050102","当前套费率2"},
            {"0004050103","当前套费率3"},
            {"0004050104","当前套费率4"},
            {"0000000000","当前组合总有功电量"},
            {"0000000100","当前组合尖有功电量"},
            {"0000000200","当前组合峰有功电量"},
            {"0000000300","当前组合平有功电量"},
            {"0000000400","当前组合谷有功电量"},
            {"0000000001","上1月组合总有功电量"},
            {"0000000101","上1月组合尖有功电量"},
            {"0000000201","上1月组合峰有功电量"},
            {"0000000301","上1月组合平有功电量"},
            {"0000000401","上1月组合谷有功电量"},
            {"0000000002","上2月组合总有功电量"},
            {"0000000102","上2月组合尖有功电量"},
            {"0000000202","上2月组合峰有功电量"},
            {"0000000302","上2月组合平有功电量"},
            {"0000000402","上2月组合谷有功电量"},
            {"0000000003","上3月组合总有功电量"},
            {"0000000004","上4月组合总有功电量"},
            {"0000000005","上5月组合总有功电量"},
            {"0000000006","上6月组合总有功电量"},
            {"0000000007","上7月组合总有功电量"},
            {"0000000008","上8月组合总有功电量"},
            {"0000000009","上9月组合总有功电量"},
            {"000000000A","上10月组合总有功电量"},
            {"000000000B","上11月组合总有功电量"},
            {"000000000C","上12月组合总有功电量"},
            {"0000010000","当前正向总有功电量"},
            {"0000010100","当前正向尖有功电量"},
            {"0000010200","当前正向峰有功电量"},
            {"0000010300","当前正向平有功电量"},
            {"0000010400","当前正向谷有功电量"},
            {"0000010001","上1月正向总有功电量"},
            {"0000010101","上1月正向尖有功电量"},
            {"0000010201","上1月正向峰有功电量"},
            {"0000010301","上1月正向平有功电量"},
            {"0000010401","上1月正向谷有功电量"},
            {"0000010002","上2月正向总有功电量"},
            {"0000010102","上2月正向尖有功电量"},
            {"0000010202","上2月正向峰有功电量"},
            {"0000010302","上2月正向平有功电量"},
            {"0000010402","上2月正向谷有功电量"},
            {"0000010003","上3月正向总有功电量"},
            {"0000010004","上4月正向总有功电量"},
            {"0000010005","上5月正向总有功电量"},
            {"0000010006","上6月正向总有功电量"},
            {"0000010007","上7月正向总有功电量"},
            {"0000010008","上8月正向总有功电量"},
            {"0000010009","上9月正向总有功电量"},
            {"000001000A","上10月正向总有功电量"},
            {"000001000B","上11月正向总有功电量"},
            {"000001000C","上12月正向总有功电量"},
            {"0000020000","当前反向总有功电量"},
            {"0000020100","当前反向尖有功电量"},
            {"0000020200","当前反向峰有功电量"},
            {"0000020300","当前反向平有功电量"},
            {"0000020400","当前反向谷有功电量"},
            {"0000020001","上1月反向总有功电量"},
            {"0000020101","上1月反向尖有功电量"},
            {"0000020201","上1月反向峰有功电量"},
            {"0000020301","上1月反向平有功电量"},
            {"0000020401","上1月反向谷有功电量"},
            {"0000020002","上2月反向总有功电量"},
            {"0000020102","上2月反向尖有功电量"},
            {"0000020202","上2月反向峰有功电量"},
            {"0000020302","上2月反向平有功电量"},
            {"0000020402","上2月反向谷有功电量"},
            {"0000020003","上3月反向总有功电量"},
            {"0000020004","上4月反向总有功电量"},
            {"0000020005","上5月反向总有功电量"},
            {"0000020006","上6月反向总有功电量"},
            {"0000020007","上7月反向总有功电量"},
            {"0000020008","上8月反向总有功电量"},
            {"0000020009","上9月反向总有功电量"},
            {"000002000A","上10月反向总有功电量"},
            {"000002000B","上11月反向总有功电量"},
            {"000002000C","上12月反向总有功电量"},
            {"0000030000","当前组合无功1总电量"},
            {"0000040000","当前组合无功2总电量"},
            {"0000050000","当前第1象限无功总电量"},
            {"0000060000","当前第2象限无功总电量"},
            {"0000070000","当前第3象限无功总电量"},
            {"0000080000","当前第4象限无功总电量"},
            {"0000030001","上1月组合无功1总电量"},
            {"0000040001","上1月组合无功2总电量"},
            {"0000050001","上1月第1象限无功总电量"},
            {"0000060001","上1月第2象限无功总电量"},
            {"0000070001","上1月第3象限无功总电量"},
            {"0000080001","上1月第4象限无功总电量"},
            {"0001010000","当前正向有功总最大需量"},
            {"0101010000","当前正向有功总最大需量发生日期"},
            {"0201010000","当前正向有功总最大需量发生时间"},
            {"0001010001","上1月正向有功总最大需量"},
            {"0101010001","上1月正向有功总最大需量发生日期"},
            {"0201010001","上1月正向有功总最大需量发生时间"},
            {"0001020000","当前反向有功总最大需量"},
            {"0101020000","当前反向有功总最大需量发生日期"},
            {"0201020000","当前反向有功总最大需量发生时间"},
            {"0001020001","上1月反向有功总最大需量"},
            {"0101020001","上1月反向有功总最大需量发生日期"},
            {"0201020001","上1月反向有功总最大需量发生时间"},
            {"0003300001","最近一次编程日期"},
            {"0103300001","最近一次编程时间"},
            {"0010000001","总失压次数"},
            {"0010000002","总失压累计时间"},
            {"0010000101","最近一次失压起始日期"},
            {"0110000101","最近一次失压起始时间"},
            {"0010000201","最近一次失压结束日期"},
            {"0110000201","最近一次失压结束时间"},
            {"0010010201","最近一次A相失压起始时刻正向有功电量"},
            {"0010012601","最近一次A相失压结束时刻正向有功电量"},
            {"0010010301","最近一次A相失压起始时刻反向有功电量"},
            {"0010012701","最近一次A相失压结束时刻反向有功电量"},
            {"0010020201","最近一次B相失压起始时刻正向有功电量"},
            {"0010022601","最近一次B相失压结束时刻正向有功电量"},
            {"0010020301","最近一次B相失压起始时刻反向有功电量"},
            {"0010022701","最近一次B相失压结束时刻反向有功电量"},
            {"0010030201","最近一次C相失压起始时刻正向有功电量"},
            {"0010032601","最近一次C相失压结束时刻正向有功电量"},
            {"0010030301","最近一次C相失压起始时刻反向有功电量"},
            {"0010032701","最近一次C相失压结束时刻反向有功电量"},
            {"",""}
        };
        #endregion

        /// <summary>
        /// 产生随机时间：HHMMSS
        /// </summary>
        /// <returns>随机时间</returns>
        public static string GenerateRandomTime()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            string ss = "";
            string mm = "";
            string hh = "";
            string hhmmss = "";
            ss = ran.Next(60).ToString("D2");
            mm = ran.Next(60).ToString("D2");
            hh = ran.Next(24).ToString("D2");

            return hhmmss = hh + mm + ss;
        }

        /// <summary>
        /// 数据处理：输入数据（包括数据，整数位数，小数位数），输出double型数据，
        /// 并且去掉前面的0
        /// </summary>
        /// <param name="recvData">输入数据</param>
        /// <param name="integerNum">整数位数</param>
        /// <param name="decimalNum">小数位数</param>
        /// <returns>Double型数据</returns>
        public static double RecvDataDeal(string recvData, int integerNum, int decimalNum)
        {
            double result = 0;
            for (int i = 0; i < integerNum; i++)
            {
                if (recvData[i] != '0')//去掉前面的0
                {
                    result = Convert.ToDouble(recvData.Substring(i, integerNum - i));
                    break;
                }
            }
            result += Convert.ToDouble(recvData.Substring(integerNum, decimalNum)) / Math.Pow(10, decimalNum);//加上小数部分
            return result;
        }

        /// <summary>
        /// 计算时间差
        /// </summary>
        /// <param name="startTime">起始时间（string格式:"HH:mm:ss"）</param>
        /// <param name="stopTime">结束时间（string格式:"HH:mm:ss"）</param>
        /// <returns>时间差(单位：小时)</returns>
        public static double CountTimeSpan(string startTime, string stopTime)
        {
            double result = 0;
            double startHH = Convert.ToDouble(startTime.Substring(0, 2));
            double startMM = Convert.ToDouble(startTime.Substring(3, 2));
            double startSS = Convert.ToDouble(startTime.Substring(6, 2));
            double stopHH = Convert.ToDouble(stopTime.Substring(0, 2));
            double stopMM = Convert.ToDouble(stopTime.Substring(3, 2));
            double stopSS = Convert.ToDouble(stopTime.Substring(6, 2));
            if (stopSS < startSS)
            {
                stopSS += 60;
                stopMM -= 1;
            }
            if (stopMM < startMM)
            {
                stopMM += 60;
                stopHH -= 1;
            }
            result = (stopHH - startHH) + (stopMM - startMM) / 60 + (stopSS - startSS) / 3600;
            return result;
        }

        /// <summary>
        /// 输入日期判断星期
        /// </summary>
        /// <param name="date">输入日期（yymmdd）</param>
        /// <returns>日期+星期（yymmddww[ww:00~06]）</returns>
        public static string JustWeek(string date)
        {
            int y = 2000 + Convert.ToInt16(date.Substring(0, 2));
            int m = Convert.ToInt16(date.Substring(2, 2));
            int d = Convert.ToInt16(date.Substring(4, 2));
            DateTime datetime = new DateTime(y, m, d);
            DayOfWeek week = datetime.DayOfWeek;
            switch (week)
            {
                case DayOfWeek.Sunday: date += "00"; break;
                case DayOfWeek.Monday: date += "01"; break;
                case DayOfWeek.Tuesday: date += "02"; break;
                case DayOfWeek.Wednesday: date += "03"; break;
                case DayOfWeek.Thursday: date += "04"; break;
                case DayOfWeek.Friday: date += "05"; break;
                case DayOfWeek.Saturday: date += "06"; break;
            }
            return date;
        }

        /// <summary>
        /// 判断字符串里是否为纯数字，是则返回true，不是则返回false
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>true:纯数字； false:不全是数字</returns>
        public static bool IsNum(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            foreach (char c in str)
            {
                if (!Char.IsNumber(c)) return false;
            }
            return true;
        }

        public static int CountTxtByte(string txt)//计字节数
        {
            txt = txt.Replace(" ", "").Replace("\r\n", "");
            return txt.Length / 2;
        }

        #region 延时函数
        [DllImport("kernel32.dll")]
        static extern uint GetTickCount();
        public static void Delay(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
            {
                Application.DoEvents();
            }
        }
        #endregion
        //static AutoResetEvent MyDelayEvent = new AutoResetEvent(false);
        //public static void Delay(uint ms)
        //{
        //    System.Timers.Timer MyDelayTimer = new System.Timers.Timer(ms);
        //    //调用延迟函数，设置和启动延时定时器，然后等待。
        //    //MyDelayTimer.Interval = ms;
        //    MyDelayTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
        //    MyDelayTimer.AutoReset = true;//每到指定时间Elapsed事件是触发一次（false），还是一直触发（true），要用true会复位时间。
        //    MyDelayTimer.Enabled = true;//是否触发Elapsed事件
        //    MyDelayTimer.Start();
            
        //    //MyDelayTimer.WaitOne();
        //    MyDelayTimer.Dispose();

        //}

        //private static void Timer_TimesUp(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    MyDelayEvent.Set();
        //}
    }
}
