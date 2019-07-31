namespace Funani.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Input;
    using Funani.Api;

    public class FunaniCommandQueue : ICommandQueue
    {
        private readonly Queue<ICommand> _actions;
        private int _processed;
        private Thread _thread;

        public FunaniCommandQueue()
        {
            _thread = null;
            _actions = new Queue<ICommand>();
        }

        public void AddCommand(ICommand action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
                if (_thread == null)
                {
                    _thread = new Thread(Worker);
                    _thread.Start();
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_actions)
                {
                    return _actions.Count + _processed;
                }
            }
        }

        public event EventHandler ThreadStarted;
        public event EventHandler ThreadEnded;
        public event EventHandler<CommandProgressEventArgs> CommandStarted;
        public event EventHandler<CommandProgressEventArgs> CommandEnded;

        private void Worker(object parameter)
        {
            try
            {
                Setup();

                while (_actions.Count > 0)
                {
                    ICommand command = _actions.Peek();
                    CommandStarted?.Invoke(this, new CommandProgressEventArgs(command));

                    command.Execute(null);

                    lock (_actions)
                    {
                        _processed++;
                        _actions.Dequeue();
                    }

                    CommandEnded?.Invoke(this, new CommandProgressEventArgs(command));
                }
            }
            finally
            {
                TearDown();
            }
        }

        private void TearDown()
        {
            _thread = null;
            ThreadEnded?.Invoke(this, null);
        }

        private void Setup()
        {
            _processed = 0;
            ThreadStarted?.Invoke(this, null);
        }
    }
}