using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNamespace.Commands
{
    public class GenericCommand<T> : BaseCommand<T>

    {
        private Action<T, object> _OnExecute;
        Func<T, object, bool> _CanExecute;
        public GenericCommand(T context, Action<T, object> onExecute, Func<T, object, bool> canExecute = null) : base(context)
        {
            _OnExecute = onExecute;
            _CanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        protected override bool OnCanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute(Context, parameter);
        }
        protected override void OnExecute(object parameter)
        {

            _OnExecute?.Invoke(Context, parameter);

        }
    }
}
