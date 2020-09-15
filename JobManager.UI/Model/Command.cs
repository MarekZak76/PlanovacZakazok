using System;
using System.Windows.Input;

namespace JobManager.UI
{
    public class Command : ICommand
    {
        private readonly Action execute;
        private readonly Action<object> executeWithParam;
        private readonly Func<bool> canExecute;

        public Command(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public Command(Action<object> execute, Func<bool> canExecute)
        {
            this.executeWithParam = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return canExecute();
        }
        public virtual void Execute(object parameter)
        {
            if (parameter == null)
                execute();
            else
                executeWithParam(parameter);
        }      
       

    }

}
