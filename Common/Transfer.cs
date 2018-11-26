using System;

namespace Common
{
    public static class Transfer
    {
        /// <summary>
        /// 返回数组的HEX字符串
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] b)
        {
            string str = "";
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

        /// <summary>
        /// 返回字节byte的HEX形式string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToHexString(this byte b)
        {
            return b.ToString("X2");
        }

        public static string ReverseString(string str)
        {
            string reStr = "";
            for (int i = 0; i < str.Length / 2; i++)
            {
                reStr += str.Substring(str.Length - (i + 1) * 2, 2);
            }
            return reStr;
        }

        /// <summary>
        /// 反转字符串（例如“123456”-->“563412”）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReverseStr(this string str)
        {
            string reStr = "";
            for (int i = 0; i < str.Length / 2; i++)
            {
                reStr += str.Substring(str.Length - (i + 1) * 2, 2);
            }
            return reStr;
        }

        public static void StrConvertToByte(string text, out byte[] buffer)
        {
            int len = text.Length;
            buffer = new byte[len / 2];
            int cnt = 0;
            if ((len & 0x01) == 0x01)
            {
                len--;
            }
            for (int i = 0; i < len; i += 2)
            {
                string str = text.Substring(i, 2);
                buffer[cnt++] = System.Convert.ToByte(str, 16);
            }
        }

        public static byte[] StrToByte(string text)
        {
            text = text.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            int len = text.Length;
            byte[] buffer = new byte[len / 2];
            int cnt = 0;
            if ((len & 0x01) == 0x01)
            {
                len--;
            }
            for (int i = 0; i < len; i += 2)
            {
                string str = text.Substring(i, 2);
                buffer[cnt++] = System.Convert.ToByte(str, 16);
            }
            return buffer;
        }

        /// <summary>
        /// Accii码转字符串输出
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AsciiToString(string str)
        {
            string strResult = null;
            for (int i = 0; i < (str.Length - 1); i += 2)
            {
                int intTemp = Convert.ToInt32(str.Substring(i, 2), 16);
                if (intTemp == 0) { strResult += "□"; continue; }
                if (intTemp == 32) { strResult += "△"; continue; }
                strResult += Char.ConvertFromUtf32(intTemp);
            }
            return strResult;
        }
    }
}
