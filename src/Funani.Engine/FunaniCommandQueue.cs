

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using Funani.Api;

namespace Funani.Engine
{
    public class FunaniCommandQueue : ICommandQueue
    {
        private readonly Queue<ICommand> _actions;
        private Int32 _processed;
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

        public Int32 Count
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
                    EventHandler<CommandProgressEventArgs> handler = CommandStarted;
                    if (handler != null)
                        handler(this, new CommandProgressEventArgs(command));

                    command.Execute(null);

                    lock (_actions)
                    {
                        _processed++;
                        _actions.Dequeue();
                    }

                    handler = CommandEnded;
                    if (handler != null)
                        handler(this, new CommandProgressEventArgs(command));
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
            EventHandler handler = ThreadEnded;
            if (handler != null)
                handler(this, null);
        }

        private void Setup()
        {
            _processed = 0;
            EventHandler handler = ThreadStarted;
            if (handler != null)
                handler(this, null);
        }
    }
}