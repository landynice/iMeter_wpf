using System.Windows.Controls;
using iMeter.ViewModel;

namespace iMeter.View.Pages
{
    /// <summary>
    /// Page_MutlRWandDisp.xaml 的交互逻辑
    /// </summary>
    public partial class Page_MutlRWandDisp : Page
    {
        public Page_MutlRWandDisp()
        {
            InitializeComponent();
            Name = "多项读写和显示";
            DataContext = new Page_MutlRWandDispViewModel();
        }
    }
}
