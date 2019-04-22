
namespace Funani.Api
{
	using System;
	using System.Windows.Input;

    public class CommandProgressEventArgs : EventArgs
    {
        public CommandProgressEventArgs(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; private set; }
    }
	
	/// <summary>
	/// Description of ICommandQueue.
	/// </summary>
	public interface ICommandQueue
	{
        void AddCommand(ICommand action);

        Int32 Count { get; }

        event EventHandler ThreadStarted;
        event EventHandler ThreadEnded;
        event EventHandler<CommandProgressEventArgs> CommandStarted;
        event EventHandler<CommandProgressEventArgs> CommandEnded;
	}
}
