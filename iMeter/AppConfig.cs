using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace iMeter
{
    /// <summary>
    /// 配置参数
    /// </summary>
    public static class AppConfig
    {
        private static AppSettingsSection appSettings;
        private static Configuration config;
        static AppConfig()
        {
            string assemblyConfigFile = System.Reflection.Assembly.GetEntryAssembly().Location;
            config = ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);
            //获取appSettings节点    
            appSettings = (AppSettingsSection)config.GetSection("appSettings");
        }
        /// <summary>
        /// 当前程序启动路径
        /// </summary>
        public static string StartupPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }
        }
        /// <summary>
        /// 加密机IP地址
        /// </summary>
        public static string ESAMIP
        {
            get
            {
                return appSettings.Settings["ESAMIP"].Value;
            }
            set
            {
                appSettings.Settings.Remove("ESAMIP");
                appSettings.Settings.Add("ESAMIP", value);
                config.Save();
            }
        }
        /// <summary>
        /// 加密机连接端口
        /// </summary>
        public static string ESAMPort
        {
            get
            {

                return appSettings.Settings["ESAMPort"].Value;
            }
            set
            {
                appSettings.Settings.Remove("ESAMPort");
                appSettings.Settings.Add("ESAMPort", value);
                config.Save();
            }
        }
        /// <summary>
        /// 端口
        /// </summary>
        public static string ComPort
        {
            get
            {
                return appSettings.Settings["ComPort"].Value;
            }
            set
            {
                appSettings.Settings.Remove("ComPort");
                appSettings.Settings.Add("ComPort", value);
                config.Save();
            }
        }
        /// <summary>
        /// 波特率
        /// </summary>
        public static int ComBaudrate
        {
            get
            {
                string str = appSettings.Settings["ComBaudrate"].Value;
                int index = 0;
                if (int.TryParse(str, out index))
                    return index;
                else
                    return 2400;
            }
            set
            {
                appSettings.Settings.Remove("ComBaudrate");
                appSettings.Settings.Add("ComBaudrate", value.ToString());
                config.Save();
            }
        }

        /// <summary>
        /// 表地址
        /// </summary>
        public static string Address
        {
            get
            {
                return appSettings.Settings["Address"].Value;
            }
            set
            {
                appSettings.Settings.Remove("Address");
                appSettings.Settings.Add("Address", value);
                config.Save();
            }
        }

        /// <summary>
        /// 操作者代码
        /// </summary>
        public static string OperaterCode
        {
            get
            {
                return appSettings.Settings["OperaterCode"].Value;
            }
            set
            {
                appSettings.Settings.Remove("OperaterCode");
                appSettings.Settings.Add("OperaterCode", value);
                config.Save();
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public static string Password
        {
            get
            {
                return appSettings.Settings["Password"].Value;
            }
            set
            {
                appSettings.Settings.Remove("Password");
                appSettings.Settings.Add("Password", value);
                config.Save();
            }
        }
    }
}
