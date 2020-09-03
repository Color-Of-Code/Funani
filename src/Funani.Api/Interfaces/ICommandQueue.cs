namespace Funani.Api
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Description of ICommandQueue.
    /// </summary>
    public interface ICommandQueue
    {
        event EventHandler ThreadStarted;

        event EventHandler ThreadEnded;

        event EventHandler<CommandProgressEventArgs> CommandStarted;

        event EventHandler<CommandProgressEventArgs> CommandEnded;

        int Count { get; }

        void AddCommand(ICommand action);
    }
}
