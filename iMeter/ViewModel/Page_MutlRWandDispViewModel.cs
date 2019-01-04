using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Model;
using Protocol.Core;

namespace iMeter.ViewModel
{
    public class Page_MutlRWandDispViewModel : ViewModelBase
    {
        public Page_MutlRWandDispViewModel()
        {
            BtnRead = new RelayCommand(Read);
            BtnSet = new RelayCommand(Set);
        }

        /// <summary>
        /// 读按钮
        /// </summary>
        public RelayCommand BtnRead { get; set; }

        /// <summary>
        /// 设按钮
        /// </summary>
        public RelayCommand BtnSet { get; set; }

        private string _ret;
        public string Ret
        {
            get { return _ret; }
            set
            {
                _ret = value;
                RaisePropertyChanged("Ret");
            }
        }

        private string _ret1;
        public string Ret1
        {
            get { return _ret1; }
            set
            {
                _ret1 = value;
                RaisePropertyChanged("Ret1");
            }
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="obj"></param>
        private void Read(object obj)
        {
            Ret = string.Empty;
            Protocol645 p645 = new Protocol645();
            p645.ReadData(obj.ToString(), out _ret);
            //Ret = obj.ToString();
        }
        /// <summary>
        /// 设数据
        /// </summary>
        /// <param name="obj"></param>
        private void Set(object obj)
        {

        }

    }
}
