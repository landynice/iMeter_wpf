using System.Windows.Controls;
using iMeter.ViewModel;

namespace iMeter.View.Pages
{
    /// <summary>
    /// Page_Status.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Status : Page
    {
        public Page_Status()
        {
            InitializeComponent();
            Name = "状态字特征字模式字";
            DataContext = new Page_StatusViewModel();
        }
    }
}
