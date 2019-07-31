namespace Funani.Api
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Description of ICommandQueue.
    /// </summary>
    public interface ICommandQueue
    {
        void AddCommand(ICommand action);

        int Count { get; }

        event EventHandler ThreadStarted;

        event EventHandler ThreadEnded;

        event EventHandler<CommandProgressEventArgs> CommandStarted;

        event EventHandler<CommandProgressEventArgs> CommandEnded;
    }
}
