using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Protocol.Core;
using Common;

namespace iMeter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PortName
        {
            get
            {
                return IProtocol.PortName;
            }
            set
            {
                IProtocol.PortName = value;
            }
        }
		public static readonly DependencyProperty OpenCommandProperty =
	DependencyProperty.Register("OpenCommand", typeof(RoutedCommand), typeof(MainWindow), new PropertyMetadata(null));

		public RoutedCommand OpenCommand
		{
			get { return (RoutedCommand)GetValue(OpenCommandProperty); }
			set { SetValue(OpenCommandProperty, value); }
		}

		void bin_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var btn = e.Source as Button;
			this.PageContext.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
		}

		public MainWindow()
        {
            InitializeComponent();
            InitializeFace();
            ShowCurTime();

			this.OpenCommand = new RoutedCommand();
			var bin = new CommandBinding(this.OpenCommand);
			bin.Executed += bin_Executed;
			this.CommandBindings.Add(bin);
		}

        #region 时钟
        private DispatcherTimer ShowTimer;//时间控件
        private void ShowCurTime()
        {
            ShowTime();
            ShowTimer = new DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();
        }

        private void ShowCurTimer(object sender, EventArgs e)
        {
            ShowTime();
        }

        private void ShowTime()
        {
            string date = DateTime.Now.ToString("yyyy/MM/dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
			this.SbiCurrentTime.Content = date + "  " + time;
        }
        #endregion

        #region 菜单控制
        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuChange2400_Click(object sender, RoutedEventArgs e)
        {
            Protocol645 p645 = new Protocol645();
            if (p645.ChangeBaudrate("08"))
            {
                IProtocol.BaudRate = 2400;
                cbBaudRate.Text = "2400";
            }
        }

        private void menuChange9600_Click(object sender, RoutedEventArgs e)
        {
            Protocol645 p645 = new Protocol645();
            if (p645.ChangeBaudrate("20"))
            {
                IProtocol.BaudRate = 9600;
                cbBaudRate.Text = "9600";
            }
        }
        #endregion

        #region 初始化
        private void InitPorts()
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            cbPort.ItemsSource = ports;
            if (ports == null || ports.Length == 0)
            {
                MessageBox.Show("没有有效可用的串口端口，请增加串口设备再继续使用软件");
                AppendTextForegroundBrush("没有有效可用的串口端口，请增加串口设备再继续使用软件", true);
            }

            cbPort.Text = AppConfig.ComPort;
            cbBaudRate.Text = AppConfig.ComBaudrate.ToString();

            //IProtocol.PortName = cbPort.Text;
            //IProtocol.BaudRate = int.Parse(cbBaudRate.Text);
        }

        private void InitializeFace()
        {
            _cloneP645Grid = new ColumnDefinition { SharedSizeGroup = "P645Group" };
            _cloneP698Grid = new ColumnDefinition { SharedSizeGroup = "P698Group" };
            _cloneToToolboxLayerGrid = new ColumnDefinition { SharedSizeGroup = "P698Group" };

            //电表基本参数初始化
            InitPorts();

            tbAddr.Text = AppConfig.Address;
            tbOprCode.Text = AppConfig.OperaterCode;
            tbPsw.Text = AppConfig.Password;
            //通讯地址、密码、操作者代码
            Protocol645.Addr = tbAddr.Text.PadLeft(12, '0');
            Protocol645.Psw = tbPsw.Text.PadLeft(8, '0');
            Protocol645.OprCode = tbOprCode.Text.PadLeft(8, '0');
        }

		#endregion

		#region 界面操作
		private ColumnDefinition _cloneP645Grid;
		private ColumnDefinition _cloneP698Grid;
		private ColumnDefinition _cloneToToolboxLayerGrid;
		private void btn645_MouseEnter(object sender, MouseEventArgs e)
        {
            P645LayerGrid.Visibility = System.Windows.Visibility.Visible;
            P645LayerGrid.SetValue(Grid.ZIndexProperty, 2);

            if (btn698.Visibility == System.Windows.Visibility.Visible)
                P698LayerGrid.Visibility = System.Windows.Visibility.Collapsed;
            else
                P698LayerGrid.SetValue(Grid.ZIndexProperty, 1);
        }

        private void btn698_MouseEnter(object sender, MouseEventArgs e)
        {
            P698LayerGrid.Visibility = System.Windows.Visibility.Visible;
            P698LayerGrid.SetValue(Grid.ZIndexProperty, 2);

            if (btn645.Visibility == System.Windows.Visibility.Visible)
                P645LayerGrid.Visibility = System.Windows.Visibility.Collapsed;
            else
                P645LayerGrid.SetValue(Grid.ZIndexProperty, 1);
        }

        private void layer0Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btn645.Visibility == System.Windows.Visibility.Visible)
                P645LayerGrid.Visibility = System.Windows.Visibility.Collapsed;

            if (btn698.Visibility == System.Windows.Visibility.Visible)
                P698LayerGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void P645LayerPinButton_Click(object sender, RoutedEventArgs e)
        {
            if (btn645.Visibility == System.Windows.Visibility.Visible)
            {
                toolboxImage.Source = new BitmapImage(new Uri("Resources/Images/9416.png", UriKind.Relative));
                btn645.Visibility = System.Windows.Visibility.Collapsed;
                layer0Grid.ColumnDefinitions.Add(_cloneP645Grid);

                if (btn698.Visibility == System.Windows.Visibility.Collapsed)
                    P645LayerGrid.ColumnDefinitions.Add(_cloneToToolboxLayerGrid);
            }
            else
            {
                toolboxImage.Source = new BitmapImage(new Uri("Resources/Images/9416.png", UriKind.Relative));
                btn645.Visibility = System.Windows.Visibility.Visible;
                P645LayerGrid.Visibility = System.Windows.Visibility.Collapsed;
                layer0Grid.ColumnDefinitions.Remove(_cloneP645Grid);

                if (btn698.Visibility == System.Windows.Visibility.Collapsed)
                    P645LayerGrid.ColumnDefinitions.Remove(_cloneToToolboxLayerGrid);
            }
        }

        private void P698LayerPinButton_Click(object sender, RoutedEventArgs e)
        {
            if (btn698.Visibility == System.Windows.Visibility.Visible)
            {
                solutionImage.Source = new BitmapImage(new Uri("Resources/Images/9416.png", UriKind.Relative));
                btn698.Visibility = System.Windows.Visibility.Collapsed;
                layer0Grid.ColumnDefinitions.Add(_cloneP698Grid);

                if (btn645.Visibility == System.Windows.Visibility.Collapsed)
                    P645LayerGrid.ColumnDefinitions.Add(_cloneToToolboxLayerGrid);
            }
            else
            {
                solutionImage.Source = new BitmapImage(new Uri("Resources/Images/9416.png", UriKind.Relative));
                btn698.Visibility = System.Windows.Visibility.Visible;
                P698LayerGrid.Visibility = System.Windows.Visibility.Collapsed;
                layer0Grid.ColumnDefinitions.Remove(_cloneP698Grid);

                if (btn645.Visibility == System.Windows.Visibility.Collapsed)
                    P645LayerGrid.ColumnDefinitions.Remove(_cloneToToolboxLayerGrid);
            }
        }
        #endregion

        #region 信息显示操作
        /// <summary>
        /// 将数据写入RichTextBox中，且奇偶行前景为不同颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="warning">告警标志，true告警，需红色标记，否则普通信息，使用默认颜色标记</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="withTime">true带时间标签，默认true</param>
        private void AppendTextForegroundBrush(string text, bool warning = false, int fontSize = 12, bool withTime = true)
        {
            Action changedText = () =>
            {
                int count = richMsg.Document.Blocks.Count;
                if (count >= 1000)
                    richMsg.Document.Blocks.Clear();
                SolidColorBrush brush = null;
                if (warning == true)
                {
                    brush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    if (count % 2 == 0)
                    {
                        brush = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    }
                }
                Paragraph p = new Paragraph();
                p.Foreground = brush;
                p.FontSize = fontSize;
                Run Runtext = new Run((withTime ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" : "") + text);
                p.Inlines.Add(Runtext);
                richMsg.Document.Blocks.Add(p);

                richMsg.ScrollToEnd();
            };

            this.Dispatcher.Invoke(changedText);
        }
        /// <summary>
        /// 将数据写入RichTextBox中，指定行前景设和字体大小
        /// </summary>
        /// <param name="text"></param>
        /// <param name="warning">告警标志，true告警，需红色标记，否则普通信息，使用默认颜色标记</param>
        /// <param name="fontSize">字体大小</param>
        private void AppendTextForegroundBrush(string text, Color fontColor, int fontSize = 12, bool withTime = true)
        {
            Action changedText = () =>
            {
                int count = richMsg.Document.Blocks.Count;
                if (count >= 1000)
                    richMsg.Document.Blocks.Clear();

                Paragraph p = new Paragraph();
                p.Foreground = new SolidColorBrush(fontColor); ;
                p.FontSize = fontSize;
                Run Runtext = new Run((withTime ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" : "") + text);
                p.Inlines.Add(Runtext);

                //Hyperlink ddd = new Hyperlink(Runtext);
                //p.Inlines.Add(ddd);

                richMsg.Document.Blocks.Add(p);

                richMsg.ScrollToEnd();

                //richMsg.IsDocumentEnabled
            };

            this.Dispatcher.Invoke(changedText);
        }
        /// <summary>
        /// 将数据写入RichTextBox中，指定行前景设和字体大小
        /// </summary>
        /// <param name="text"></param>
        private void AppendTextParityBackground(string text, int fontSize = 15, bool withTime = true)
        {
            Action changedText = () =>
            {
                int count = richMsg.Document.Blocks.Count;
                SolidColorBrush brush = null;
                if (count % 2 == 0)
                {
                    brush = new SolidColorBrush(Colors.LightGreen);
                }
                else
                {
                    brush = new SolidColorBrush(Colors.White);
                }
                Paragraph p = new Paragraph();
                p.Background = brush;
                p.FontSize = fontSize;
                Run Runtext = new Run((withTime ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" : "") + text);
                p.Inlines.Add(Runtext);
                richMsg.Document.Blocks.Add(p);

                richMsg.ScrollToEnd();
            };

            this.Dispatcher.Invoke(changedText);
        }


        /// <summary>
        /// 将数据写入RichTextBox中，指定行前景设和字体大小
        /// 并将数据显示为超链接
        /// </summary>
        /// <param name="text"></param>
        /// <param name="warning">告警标志，true告警，需红色标记，否则普通信息，使用默认颜色标记</param>
        /// <param name="fontSize">字体大小</param>
        private void AppendTextForegroundBrushHyperlink(string text, IList<string> tag, int fontSize = 12, bool withTime = true)
        {
            Action changedText = () =>
            {
                int count = richMsg.Document.Blocks.Count;
                if (count >= 1000)
                    richMsg.Document.Blocks.Clear();

                Paragraph p = new Paragraph();
                p.FontSize = fontSize;
                if (withTime == true)
                {
                    p.Inlines.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ");
                }

                Run Runtext = new Run(text);
                Hyperlink hl = new Hyperlink(Runtext);
                hl.Click += hl_Click;
                hl.Tag = tag;
                p.Inlines.Add(hl);

                richMsg.Document.Blocks.Add(p);

                richMsg.ScrollToEnd();
            };

            this.Dispatcher.Invoke(changedText);
        }
        /// <summary>
        /// 超链接事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void hl_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Hyperlink)
            {
                Hyperlink hl = e.OriginalSource as Hyperlink;
                if (hl.Tag is IList<string>)
                {
                    IList<string> tag = hl.Tag as IList<string>;
                }
            }
        }

        #endregion

        #region 表地址
        private void btnReadAddr_Click(object sender, RoutedEventArgs e)
        {
            tbAddr.Text = string.Empty;
            System.Threading.Thread.Sleep(100);

            Protocol645 p645 = new Protocol645();
            string ret = string.Empty;
            if(p645.ReadAddress(out ret))
            {
                tbAddr.Text = ret;
            }
        }

        private void btnSetAddr_Click(object sender, RoutedEventArgs e)
        {
            if (tbAddr.Text != "")
            {
                string addr = tbAddr.Text.PadLeft(12, '0');
                if (Functions.IsNum(addr))
                {
                    Protocol645 p645 = new Protocol645();
                    p645.WriteAddress(addr);
                }
                else
                {
                    MessageBox.Show("请不要输入数字以外的字符！");
                }
            }
            else
            {
                MessageBox.Show("请输入表地址！");
            }
        }
        #endregion

        #region 窗口关闭事件
        private void WinClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                AppConfig.ComBaudrate = int.Parse(cbBaudRate.Text);
                AppConfig.ComPort = cbPort.Text;
                AppConfig.Address = tbAddr.Text;
                AppConfig.OperaterCode = tbOprCode.Text;
                AppConfig.Password = tbPsw.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        
    }
}
