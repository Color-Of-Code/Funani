namespace Funani.Api
{
    using System;
    using System.Windows.Input;

    public class CommandProgressEventArgs : EventArgs
    {
        public CommandProgressEventArgs(ICommand command)
        {
            this.Command = command;
        }

        public ICommand Command { get; private set; }
    }
}
