using System;

using Catel.Data;
using Catel.MVVM;

using Funani.Api;

namespace Funani.Gui.ViewModels
{
    public class CommandProgressViewModel : ViewModelBase
    {
        private readonly ICommandQueue _commands;

        public CommandProgressViewModel(ICommandQueue commands)
        {
            _commands = commands;
            BindEvents();
        }

        #region Property: Total
        /// <summary>
        /// Count of operations.
        /// </summary>
        public Int32 Total
        {
            get;
            set;
        }

        #endregion

        #region Property: Performed
        /// <summary>
        /// Count of already performed operations.
        /// </summary>
        public Int32 Performed
        {
            get;
            set;
        }
        #endregion

        #region Property: Info
        /// <summary>
        /// Information String.
        /// </summary>
        public String Info
        {
            get;
            set;
        }
        #endregion

        #region Property: Info
        /// <summary>
        /// Estimated time of arrival, when the current batch of operations will roughly be finished.
        /// </summary>
        public String Eta
        {
            get;
            set;
        }

        #endregion

        private void model_CommandStarted(object sender, CommandProgressEventArgs e)
        {
            Info = e.Command.ToString();
        }

        private void model_CommandEnded(object sender, CommandProgressEventArgs e)
        {
            Performed++;
            Total = _commands.Count;
            //TODO: refresh Eta
        }

        private void model_ThreadStarted(object sender, EventArgs e)
        {
            Performed = 0;
            Total = _commands.Count;
            Info = String.Empty;
            Eta = String.Empty;
        }

        private void model_ThreadEnded(object sender, EventArgs e)
        {
            Performed = Total;
            Info = String.Format("{0} actions performed", Total);
        }

        private void BindEvents()
        {
            _commands.ThreadStarted += model_ThreadStarted;
            _commands.ThreadEnded += model_ThreadEnded;
            _commands.CommandStarted += model_CommandStarted;
            _commands.CommandEnded += model_CommandEnded;
        }
    }
}