/*
 * Copyright (c) 2008-2013, Jaap de Haan <jaap.dehaan@color-of-code.de>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of the "Color-Of-Code" nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

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
        ///     Register the SelectedDirectory property so it is known in the class.
        /// </summary>
        public static readonly PropertyData SelectedDirectoryProperty =
            RegisterProperty("SelectedDirectory", typeof(DirectoryViewModel), null,
                             (sender, e) => ((DirectoryBrowserViewModel)sender)
                                 .OnSelectedDirectoryChanged(
                                     e.OldValue as DirectoryViewModel,
                                     e.NewValue as DirectoryViewModel));

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public DirectoryViewModel SelectedDirectory
        {
            get { return GetValue<DirectoryViewModel>(SelectedDirectoryProperty); }
            set { SetValue(SelectedDirectoryProperty, value); }
        }

        /// <summary>
        ///     Called when the SelectedDirectory property has changed.
        /// </summary>
        private void OnSelectedDirectoryChanged(DirectoryViewModel oldValue, DirectoryViewModel newValue)
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