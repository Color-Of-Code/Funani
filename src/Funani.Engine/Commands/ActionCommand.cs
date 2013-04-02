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