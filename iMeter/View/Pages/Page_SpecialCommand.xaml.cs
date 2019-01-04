using System.Windows.Controls;
using iMeter.ViewModel;

namespace iMeter.View.Pages
{
    /// <summary>
    /// Page_SpecialCommand.xaml 的交互逻辑
    /// </summary>
    public partial class Page_SpecialCommand : Page
    {
        public Page_SpecialCommand()
        {
            InitializeComponent();
            Name = "特殊命令";
            DataContext = new Page_SpecialCommandViewModel();
        }
    }
}
