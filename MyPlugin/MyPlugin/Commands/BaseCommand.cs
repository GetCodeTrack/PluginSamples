using System;
using System.Windows.Input;

namespace MyNamespace.Commands
{
    public class BaseCommand<T> : ICommand
    {
        public T Context { get; private set; }

        public BaseCommand(T context)
        {
            Context = context;
        }

        public void Execute(object parameter)
        {

            OnExecute(parameter);

        }

        public bool CanExecute(object parameter)
        {


            return OnCanExecute(parameter);

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected virtual void OnExecute(object parameter)
        {

        }

        protected virtual bool OnCanExecute(object parameter)
        {
            return true;
        }
    }
}
