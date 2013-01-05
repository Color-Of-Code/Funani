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
namespace Funani.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;

	using Funani.Gui.Model;

	/// <summary>
	/// Interaction logic for FileView.xaml
	/// </summary>
	public partial class FileView : UserControl
	{
		public FileView()
		{
			InitializeComponent();
			
			DataContext = this;
		}

		public string SelectedPath
		{
			get { return (string)GetValue(SelectedPathProperty); }
			set { SetValue(SelectedPathProperty, value); }
		}

        private bool _filterAlreadyStored = false;

		public void ReloadFiles()
		{
            var provider = new FileViewModelProvider(SelectedPath, _filterAlreadyStored);
			var items = new AsyncVirtualizingCollection<FileViewModel>(provider, 20, 10 * 1000);
			listControl.DataContext = items;
		}
		
		private static void OnSelectedPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var fileView = d as FileView;
			fileView.ReloadFiles();
		}
		
		private void CheckBox_Clicked(object sender, RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
            bool isChecked = false;
            if (checkBox.IsChecked.HasValue)
            {
                isChecked = (bool)(checkBox.IsChecked);
            }

            var viewModel = checkBox.DataContext as FileViewModel;
            var viewModels = new List<FileViewModel>();

            if (listControl.SelectedItem == null || !listControl.SelectedItems.Contains(viewModel))
			{
                viewModels.Add(viewModel);
			}
			else
			{
                viewModels.AddRange(listControl.SelectedItems.Cast<FileViewModel>());
			}

            Thread backgroundThread;
            if (isChecked)
                backgroundThread = new Thread(SetInsideFunani);
            else
                backgroundThread = new Thread(ResetInsideFunani);
            backgroundThread.IsBackground = true;
            backgroundThread.Start(viewModels);
        }

        private static void ResetInsideFunani(object parameter)
        {
            var viewModels = parameter as IList<FileViewModel>;
            foreach (FileViewModel item in viewModels)
                item.InsideFunani = null;
            foreach (FileViewModel item in viewModels)
                item.InsideFunani = false;
        }

        private static void SetInsideFunani(object parameter)
        {
            var viewModels = parameter as IList<FileViewModel>;
            foreach (FileViewModel item in viewModels)
                item.InsideFunani = null;
            foreach (FileViewModel item in viewModels)
                item.InsideFunani = true;
        }

		public static readonly DependencyProperty SelectedPathProperty =
			DependencyProperty.Register("SelectedPath", typeof(string), typeof(FileView),
			                            new PropertyMetadata(string.Empty, OnSelectedPathChanged));
	}
}