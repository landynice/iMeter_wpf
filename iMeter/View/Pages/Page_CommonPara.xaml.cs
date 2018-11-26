using System.Windows.Controls;
using iMeter.ViewModel;

namespace iMeter.View.Pages
{
    /// <summary>
    /// Page_CommonPara.xaml 的交互逻辑
    /// </summary>
    public partial class Page_CommonPara : Page
    {
        public Page_CommonPara()
        {
            InitializeComponent();
			Name = "常用参数";
			DataContext = new Page_CommonParaViewModel();
		}
    }
}
