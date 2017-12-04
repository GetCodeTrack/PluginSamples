using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeTrack.Plugins
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
            try
            {
                OnExecute(parameter);
            }
            catch (Exception ex)
            {
                //((IShowException)Application.Current).ShowException(ex);
            }

        }

        public bool CanExecute(object parameter)
        {
            try
            {
                return OnCanExecute(parameter);
            }
            catch (Exception ex)
            {
                //((IShowException)Application.Current).ShowException(ex);
            }
            return false;
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
