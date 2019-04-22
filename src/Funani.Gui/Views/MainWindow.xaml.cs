
using Catel.Windows;
using Funani.Gui.Helpers.Views;

namespace Funani.Gui.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DataWindow
    {
#if DEBUG
        private readonly TraceOutputWindow _traceOutputWindow;
#endif

        public MainWindow()
            : base(DataWindowMode.Custom)
        {
            InitializeComponent();

#if DEBUG
            _traceOutputWindow = new TraceOutputWindow();
            _traceOutputWindow.Show();

            Closed += (sender, e) =>
            {
                if (_traceOutputWindow != null)
                {
                    _traceOutputWindow.Close();
                }
            };
#endif
        }
    }
}