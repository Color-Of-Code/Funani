using Catel.Windows;
using System;

namespace Funani.Gui.Helpers.Views
{
    /// <summary>
    /// Interaction logic for TraceOutputWindow.xaml
    /// </summary>
    public partial class TraceOutputWindow : DataWindow
    {
        public TraceOutputWindow()
            : base(DataWindowMode.Custom, setOwnerAndFocus: false)
        {
            InitializeComponent();
        }
    }
}
