namespace Funani.Api
{
    using System;

    /// <summary>
    /// Callbacks to redirect console output and error to a listener.
    /// </summary>
    public interface IConsoleRedirect
    {
        void OnOutputDataReceived(string data);

        void OnErrorDataReceived(string data);
    }
}
