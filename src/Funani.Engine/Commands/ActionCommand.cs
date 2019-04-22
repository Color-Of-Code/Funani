
using System;
using System.Windows.Input;

namespace Funani.Engine.Commands
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;
        private bool _isEnabled;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public ActionCommand(Action action, String description)
            : this(action)
        {
            Description = description;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    TriggerCanExecuteChanged();
                }
            }
        }

        public String Description { get; protected set; }

        public bool CanExecute(object parameter)
        {
            return _isEnabled;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;

        public override string ToString()
        {
            return string.Format("{0}", Description);
        }

        private void TriggerCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}