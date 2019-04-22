
using Catel.IoC;
using Catel.MVVM;
using Catel.Windows.Controls;
using Funani.Api;
using Funani.Gui.ViewModels;

namespace Funani.Gui.Views
{
    /// <summary>
    ///     Interaction logic for MongoDbView.xaml
    /// </summary>
    public partial class MongoDbView : UserControl
    {
        public MongoDbView()
        {
            InitializeComponent();
        }

        /*FIXME: find alternative
        protected override IViewModel GetViewModelInstance(object dataContext)
        {
            var viewModel = ServiceLocator.Default.ResolveType<IConsoleRedirect>() as MongoDbViewModel;
            DataContext = viewModel;
            return viewModel;
        }
         */
    }
}