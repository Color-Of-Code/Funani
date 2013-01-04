/*
 * Copyright (c) 2008-2013, Jaap de Haan <jaap.dehaan@color-of-code.de>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of the "Color-Of-Code" nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace Funani.Gui.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Input;

    public class CommandProgressEventArgs : EventArgs
    {
        public CommandProgressEventArgs(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; private set; }
    }

    public class CommandProgressModel
    {
        public CommandProgressModel()
        {
            _thread = null;
            _actions = new Queue<ICommand>();
        }

        public void AddCommand(ICommand action)
        {
            _actions.Enqueue(action);
            if (_thread == null)
            {
                _thread = new Thread(Worker);
                _thread.Start();
            }
        }

        public Int32 Count
        {
            get
            {
                return _actions.Count;
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
                    var handler = CommandStarted;
                    if (handler != null)
                        handler(this, new CommandProgressEventArgs(command));

                    command.Execute(null);

                    handler = CommandEnded;
                    if (handler != null)
                        handler(this, new CommandProgressEventArgs(command));
                    _actions.Dequeue();
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
            var handler = ThreadEnded;
            if (handler != null)
                handler(this, null);
        }

        private void Setup()
        {
            var handler = ThreadStarted;
            if (handler != null)
                handler(this, null);
        }

        private Thread _thread;
        private Queue<ICommand> _actions;
    }
}
