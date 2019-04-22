
using Catel.Windows.Controls;
using Funani.Gui.ViewModels;

namespace Funani.Gui.Views
{
    /// <summary>
    ///     Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        public DatabaseView()
        {
            InitializeComponent();

            TokenizerPeople.TokenMatcher = DatabaseViewModel.TokenMatcher;
            TokenizerLocation.TokenMatcher = DatabaseViewModel.TokenMatcher;
            TokenizerEvent.TokenMatcher = DatabaseViewModel.TokenMatcher;
            TokenizerKeywords.TokenMatcher = DatabaseViewModel.TokenMatcher;
        }
    }
}