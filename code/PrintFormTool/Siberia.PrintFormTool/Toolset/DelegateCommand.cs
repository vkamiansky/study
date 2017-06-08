using System;
using System.Windows.Input;

namespace Siberia.PrintFormTool.Toolset
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };

        private readonly Action<object> execute;

        private readonly Predicate<object> canExecute;

        public DelegateCommand(Action<object> execute) : this(execute, null)
        {  
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
