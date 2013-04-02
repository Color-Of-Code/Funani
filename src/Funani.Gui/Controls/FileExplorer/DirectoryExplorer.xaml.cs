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

using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Funani.Api;
using Funani.Engine.Commands;

namespace Funani.Gui.Controls.FileExplorer
{
    /// <summary>
    ///     Interaction logic for FileExplorer.xaml
    /// </summary>
    public partial class DirectoryExplorer : UserControl
    {
        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof (string), typeof (DirectoryExplorer),
                                        new PropertyMetadata(string.Empty));

        private DirectoryTreeViewModel _viewModel;

        public DirectoryExplorer()
        {
            InitializeComponent();

        }

        public IEngine FunaniEngine
        {
            set
            {
                _viewModel = new DirectoryTreeViewModel(value);
                Directories.DataContext = _viewModel;
            }
        }

        public string SelectedPath
        {
            get { return (string) GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        public void SelectPath(string path)
        {
            SelectedPath = path;
            if (_viewModel != null)
                _viewModel.ExpandAndSelect(new DirectoryInfo(path));
        }


        private void directories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DirectoryViewModel item = SelectedDirectoryViewModel();
            if (item != null)
            {
                DirectoryInfo di = item.DirectoryInfo;
                if (di != null)
                {
                    SelectedPath = di.FullName;
                }
            }
        }

        private void UploadAllFiles_Click(object sender, RoutedEventArgs e)
        {
            DirectoryViewModel item = SelectedDirectoryViewModel();
            _viewModel.UploadAllFiles(item);
        }

        private void UploadAllFilesRecursively_Click(object sender, RoutedEventArgs e)
        {
            DirectoryViewModel item = SelectedDirectoryViewModel();
            _viewModel.UploadAllFilesRecursively(item);
        }

        private DirectoryViewModel SelectedDirectoryViewModel()
        {
            var item = (DirectoryViewModel) Directories.SelectedItem;
            return item;
        }
    }
}