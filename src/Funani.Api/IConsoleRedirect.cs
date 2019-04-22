
namespace Funani.Api
{
    using System;

    /// <summary>
    /// Callbacks to redirect console output and error to a listener
    /// </summary>
    public interface IConsoleRedirect
    {
        void OnOutputDataReceived(String data);
        void OnErrorDataReceived(String data);
    }
}
