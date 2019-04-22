
using Catel.Windows.Controls;
using Funani.Gui.ViewModels;

namespace Funani.Gui.Views
{
    /// <summary>
    ///     Interaction logic for FileInformationView.xaml
    /// </summary>
    public partial class FileInformationView : UserControl
    {
        public FileInformationView()
        {
            InitializeComponent();

            TokenizerPeople.TokenMatcher = DatabaseViewModel.TokenMatcher;
            TokenizerLocation.TokenMatcher = DatabaseViewModel.TokenMatcher;
            TokenizerEvent.TokenMatcher = DatabaseViewModel.TokenMatcher;
            TokenizerKeywords.TokenMatcher = DatabaseViewModel.TokenMatcher;
        }
    }
}