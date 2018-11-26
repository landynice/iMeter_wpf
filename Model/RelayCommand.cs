using System;
using System.Windows.Input;

namespace Model
{
    public class RelayCommand : ICommand
    {
        public Action ExecuteAction; //执行方法
        public Action<object> ExecuteCommand; //执行方法 带参数
        public Func<object, bool> CanExecuteCommand; //执行方法的条件
        public RelayCommand(Action action)// 执行事件
        {
            ExecuteAction = action;
        }
        public RelayCommand(Action<object> action)// 执行带参数的事件
        {
            ExecuteCommand = action;
        }
        public RelayCommand(Action<object> action, Func<object, bool> can)// 根据条件执行带参数的事件
        {
            ExecuteCommand = action;
            CanExecuteCommand = can;
        }
        public event EventHandler CanExecuteChanged;// 当命令可执行状态发生改变时，应当被激发
        public bool CanExecute(object parameter)// 用于判断命令是否可以执行
        {
            if (ExecuteAction != null) return true;
            return CanExecuteCommand == null || CanExecuteCommand(parameter);
        }
        public void Execute(object parameter)//命令执行
        {
            if (ExecuteCommand != null) ExecuteCommand(parameter);
            else ExecuteAction();
        }
    }
}
