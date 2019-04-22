
using System.Windows;
using System.Windows.Controls;
using Funani.Gui.ViewModels;
using UserControl = Catel.Windows.Controls.UserControl;

namespace Funani.Gui.Views
{
    /// <summary>
    ///     Interaction logic for DirectoryTreeView.xaml
    /// </summary>
    public partial class DirectoryTreeView : UserControl
    {
        public DirectoryTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var v = sender as TreeView;
            var item = v.SelectedItem as DirectoryViewModel;
            var vm = ViewModel as DirectoryTreeViewModel;
            if (vm != null)
            {
                vm.SelectedDirectory = item;
            }
        }
    }
}