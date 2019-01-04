using System.Windows.Controls;
using iMeter.ViewModel;

namespace iMeter.View.Pages
{
    /// <summary>
    /// Page_ESAM_Ctrl.xaml 的交互逻辑
    /// </summary>
    public partial class Page_ESAM_Ctrl : Page
    {
        public Page_ESAM_Ctrl()
        {
            InitializeComponent();
            Name = "ESAM费控";
            DataContext = new Page_ESAM_CtrlViewModel();
        }
    }
}
