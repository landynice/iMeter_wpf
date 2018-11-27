using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Protocol.Core;
using Common;

namespace iMeter.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
		{
            BtnReadAddr = new RelayCommand(ReadAddr);
            BtnSetAddr = new RelayCommand(SetAddr);
			MiClose = new RelayCommand(CloseWindow);
			MiChangeBaudRate = new RelayCommand(ChangeBaudRate);
		}

        /// <summary>
        /// 读表地址按钮
        /// </summary>
        public RelayCommand BtnReadAddr { get; set; }

        /// <summary>
        /// 设表地址按钮
        /// </summary>
        public RelayCommand BtnSetAddr { get; set; }

		/// <summary>
		/// 关闭菜单
		/// </summary>
		public RelayCommand MiClose { get; set; }

		/// <summary>
		/// 更改波特率菜单
		/// </summary>
		public RelayCommand MiChangeBaudRate { get; set; }

		/// <summary>
		/// 电表地址
		/// </summary>
		public string Address
        {
            get { return Protocol645.Addr; }
            set
            {
                Protocol645.Addr = value;
                RaisePropertyChanged("Address");
            }
        }

        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName
        {
            get{ return IProtocol.PortName; }
            set
            {
                IProtocol.PortName = value;
                RaisePropertyChanged("PortName");
            }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public int Baudrate
        {
            get => IProtocol.BaudRate;
            set
            {
                IProtocol.BaudRate = value;
                RaisePropertyChanged("Baudrate");
            }
        }

		/// <summary>
		/// 操作者代码
		/// </summary>
		public string OperCode
		{
			get { return Protocol645.OprCode; }
			set
			{
				Protocol645.OprCode = value;
				RaisePropertyChanged("OperCode");
			}
		}

		/// <summary>
		/// 密码
		/// </summary>
		public string Psw
		{
			get { return Protocol645.Psw; }
			set
			{
				Protocol645.Psw = value;
				RaisePropertyChanged("Psw");
			}
		}

        /// <summary>
        /// 读表地址
        /// </summary>
        /// <param name="obj"></param>
        private void ReadAddr(object obj)
        {
            Address = string.Empty;

            Protocol645 p645 = new Protocol645();
            string ret = string.Empty;
            if (p645.ReadAddress(out ret))
            {
                Address = ret;
            }
        }

        /// <summary>
        /// 设表地址
        /// </summary>
        /// <param name="obj"></param>
        private void SetAddr(object obj)
        {
            if (Address != string.Empty)
            {
                Address = Address.PadLeft(12, '0');
                if (Functions.IsNum(Address))
                {
                    Protocol645 p645 = new Protocol645();
                    p645.WriteAddress(Address);
                }
            }
        }

		/// <summary>
		/// 关闭窗口
		/// </summary>
		/// <param name="obj"></param>
		private void CloseWindow(object obj)
		{
			System.Windows.Application.Current.Shutdown();
		}

		/// <summary>
		/// 更改波特率
		/// </summary>
		/// <param name="obj"></param>
		private void ChangeBaudRate(object obj)
		{
			Protocol645 p645 = new Protocol645();
			string baud = string.Empty;
			if(obj.ToString() == "2400")
			{
				baud = "08";
			}
			if(obj.ToString() == "9600")
			{
				baud = "20";
			}
			if (p645.ChangeBaudrate(baud))
			{
				int b = Convert.ToInt32(obj.ToString());
				IProtocol.BaudRate = b;
				Baudrate = b;
			}
		}
	}
}
