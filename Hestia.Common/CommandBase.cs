using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hestia.Common
{
    /// <summary>
    /// Třída implementující rozhraní ICommand
    /// </summary>
    public class CommandBase : ICommand
    {
        public CommandBase(Action<object> execute)
        : this(execute, null)
        { }

        public CommandBase(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : true;
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
        }
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
        private readonly Action<object> _execute = null;
        private readonly Predicate<object> _canExecute = null;
    }
}
