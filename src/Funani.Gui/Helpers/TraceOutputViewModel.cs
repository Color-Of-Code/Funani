using System.Windows;

using Catel.Data;
using Catel.MVVM;

using System.ComponentModel;

namespace Funani.Gui.Helpers
{
    /// <summary>
    ///     TraceOutput view model.
    /// </summary>
    public class TraceOutputViewModel : ViewModelBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TraceOutputViewModel" /> class.
        /// </summary>
        public TraceOutputViewModel()
        {
            Left = SystemParameters.PrimaryScreenWidth - Width - 15;
            Top = SystemParameters.PrimaryScreenHeight - Height - 40;
        }

        public override string Title
        {
            get { return "TraceOutput"; }
        }

        #region Property: Top

        public double Top
        {
            get;
            set;
        }

        #endregion

        #region Property: Left

        public double Left
        {
            get;
            set;
        }

        #endregion

        #region Property: Width

        [DefaultValue(320.0)]
        public double Width
        {
            get;
            set;
        }

        #endregion

        #region Property: Height

        [DefaultValue(240.0)]
        public double Height
        {
            get;
            set;
        }

        #endregion
    }
}