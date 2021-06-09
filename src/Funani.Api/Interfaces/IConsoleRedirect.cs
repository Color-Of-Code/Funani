namespace Funani.Api
{
    /// <summary>
    /// Callbacks to redirect console output and error to a listener.
    /// </summary>
    public interface IConsoleRedirect
    {
        /// <summary>
        /// Callback for standard output.
        /// </summary>
        /// <param name="data">the data.</param>
        void OnOutputDataReceived(string data);

        /// <summary>
        /// Callback for standard error.
        /// </summary>
        /// <param name="data">the data.</param>
        void OnErrorDataReceived(string data);
    }
}
