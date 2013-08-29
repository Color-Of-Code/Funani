using Catel.Logging;

namespace Funani.Gui.Logging
{
	public class Log4NetListener : LogListenerBase
	{
		private readonly log4net.ILog _log = log4net.LogManager.GetLogger("UUM");
		
		protected override void Debug(ILog log, string message, object extraData)
		{
			_log.Debug(message);
		}
		
		protected override void Info(ILog log, string message, object extraData)
		{
			_log.Info(message);
		}
		
		protected override void Warning(ILog log, string message, object extraData)
		{
			_log.Warn(message);
		}
		
		protected override void Error(ILog log, string message, object extraData)
		{
			_log.Error(message);
		}
	}
}
