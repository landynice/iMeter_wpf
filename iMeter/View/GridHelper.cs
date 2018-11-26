using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iMeter.View.Pages
{
	public class GridHelper
	{

		//请注意：可以通过propa这个快捷方式生成下面三段代码

		public static bool GetShowBorder(DependencyObject obj)
		{
			return (bool)obj.GetValue(ShowBorderProperty);
		}

		public static void SetShowBorder(DependencyObject obj, bool value)
		{
			obj.SetValue(ShowBorderProperty, value);
		}

		public static readonly DependencyProperty ShowBorderProperty =
			DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridHelper), new PropertyMetadata(OnShowBorderChanged));


		//这是一个事件处理程序，需要手工编写，必须是静态方法
		private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = d as Grid;
			if ((bool)e.OldValue)
			{
				grid.Loaded -= (s, arg) => { };
			}
			if ((bool)e.NewValue)
			{
				grid.Loaded += (s, arg) =>
				{

					//这种做法自动将控件移动到Border里面来
					var controls = grid.Children;
					var count = controls.Count;

					for (int i = 0; i < count; i++)
					{
						var item = controls[i] as FrameworkElement;
						var border = new Border()
						{
							BorderBrush = new SolidColorBrush(Colors.LightGray),
							BorderThickness = new Thickness(1),
							Padding = new Thickness(0)//设置控件到边界距离
						};

						var row = Grid.GetRow(item);
						var column = Grid.GetColumn(item);
						var rowspan = Grid.GetRowSpan(item);
						var columnspan = Grid.GetColumnSpan(item);

						Grid.SetRow(border, row);
						Grid.SetColumn(border, column);
						Grid.SetRowSpan(border, rowspan);
						Grid.SetColumnSpan(border, columnspan);


						grid.Children.RemoveAt(i);
						border.Child = item;
						grid.Children.Insert(i, border);

					}
				};
			}

		}

	}

}
