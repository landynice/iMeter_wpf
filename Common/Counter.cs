
namespace Common
{
    public static class Counter
    {
        /// <summary>
        /// -33操作
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>string格式数据</returns>
        public static string Sub33H(byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                b[i] -= 0x33;
                b[i] = (byte)(b[i] % 256);
            }
            return b.ToHexString();
        }

        public static byte ByteSub33H(byte b)
        {
            return (byte)((b - 0x33) % 256);
        }

        public static string Sum(string str)
        {
            byte temp = 0;
            string result = null;
            for (int i = 0; i < str.Length / 2; i++)
            {
                temp += System.Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
            result = temp.ToString("X2");
            result = result.Substring(result.Length - 2, 2);
            return result;
        }

        /// <summary>
        /// +33操作
        /// </summary>
        /// <param name="str">string类型参数</param>
        /// <returns>string类型结果</returns>
        public static string StringAdd33(string str)
        {
            if (str != "")
            {
                byte[] buf;
                buf = new byte[str.Length / 2];
                string retStr = null;
                for (int i = 0; i < str.Length / 2; i++)
                {
                    buf[i] = System.Convert.ToByte(str.Substring(i * 2, 2), 16);
                    buf[i] += 0x33;
                    buf[i] = (byte)(buf[i] % 256);
                    retStr += buf[i].ToString("X2");
                }
                return retStr;
            }
            else return "";
        }

        public static int CountTxtByte(string txt)//计字节数
        {
            txt = txt.Replace(" ", "").Replace("\r\n", "");
            return txt.Length / 2;
        }
    }
}
