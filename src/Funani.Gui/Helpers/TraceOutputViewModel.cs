using System.Windows;
using Catel.Data;
using Catel.MVVM;

namespace Funani.Gui.Helpers
{
    /// <summary>
    /// TraceOutput view model.
    /// </summary>
    public class TraceOutputViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOutputViewModel"/> class.
        /// </summary>
        public TraceOutputViewModel()
        {
            Left = SystemParameters.PrimaryScreenWidth - Width - 15;
            Top = SystemParameters.PrimaryScreenHeight - Height - 40;
        }

        public override string Title { get { return "TraceOutput"; } }

        #region Property: Top
        public double Top
        {
            get { return GetValue<double>(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        public static readonly PropertyData TopProperty =
            RegisterProperty("Top", typeof(double), null);
        #endregion

        #region Property: Left
        public double Left
        {
            get { return GetValue<double>(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public static readonly PropertyData LeftProperty =
            RegisterProperty("Left", typeof(double), null);
        #endregion

        #region Property: Width
        public double Width
        {
            get { return GetValue<double>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly PropertyData WidthProperty =
            RegisterProperty("Width", typeof(double), 320.0);
        #endregion

        #region Property: Height
        public double Height
        {
            get { return GetValue<double>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly PropertyData HeightProperty =
            RegisterProperty("Height", typeof(double), 240.0);
        #endregion
    }
}
