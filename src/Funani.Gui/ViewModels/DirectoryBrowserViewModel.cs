
using System.Collections.Generic;
using System.Diagnostics;
using Catel.Data;
using Catel.MVVM;
using Funani.Gui.Controls;
using Funani.Gui.Model;

namespace Funani.Gui.ViewModels
{
    [InterestedIn(typeof(DirectoryTreeViewModel))]
    public class DirectoryBrowserViewModel : ViewModelBase
    {
        protected override void OnViewModelPropertyChanged(IViewModel viewModel, string propertyName)
        {
            base.OnViewModelPropertyChanged(viewModel, propertyName);
            if (propertyName == "SelectedDirectory")
                SelectedDirectory = (viewModel as DirectoryTreeViewModel).SelectedDirectory;
        }

        //private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        //{
        //    var checkBox = sender as CheckBox;
        //    Debug.Assert(checkBox != null, "checkBox != null");
        //    bool isChecked = checkBox.IsChecked ?? false;

        //    var viewModel = checkBox.DataContext as FileViewModel;
        //    var viewModels = new List<FileViewModel>();

        //    if (ListControl.SelectedItem == null || !ListControl.SelectedItems.Contains(viewModel))
        //    {
        //        viewModels.Add(viewModel);
        //    }
        //    else
        //    {
        //        viewModels.AddRange(ListControl.SelectedItems.Cast<FileViewModel>());
        //    }

        //    Thread backgroundThread = isChecked ? new Thread(Store) : new Thread(Remove);
        //    backgroundThread.IsBackground = true;
        //    backgroundThread.Start(viewModels);
        //}

        private static void Remove(object parameter)
        {
            var viewModels = parameter as IList<FileViewModel>;
            Debug.Assert(viewModels != null, "viewModels != null");
            foreach (FileViewModel item in viewModels)
                item.Remove.Execute();
        }

        private static void Store(object parameter)
        {
            var viewModels = parameter as IList<FileViewModel>;
            Debug.Assert(viewModels != null, "viewModels != null");
            foreach (FileViewModel item in viewModels)
                item.Store.Execute();
        }

        #region Property: SelectedDirectory

        private const bool FilterAlreadyStored = false;

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public DirectoryViewModel SelectedDirectory
        {
            get;
            set;
        }

        /// <summary>
        ///     Called when the SelectedDirectory property has changed.
        /// </summary>
        private void OnSelectedDirectoryChanged()
        {
            var provider = new FileViewModelProvider(SelectedDirectory.DirectoryInfo.FullName, FilterAlreadyStored);
            var items = new AsyncVirtualizingCollection<FileViewModel>(provider, 20, 10 * 1000);
            FileViewModels = items;
        }

        #endregion

        #region Property: FileViewModels

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public IEnumerable<FileViewModel> FileViewModels { get; set; }

        #endregion
    }
}