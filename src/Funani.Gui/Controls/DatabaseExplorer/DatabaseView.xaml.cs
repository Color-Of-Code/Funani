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
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;

    using Funani.Gui.Model;

    /// <summary>
	/// Interaction logic for DatabaseView.xaml
	/// </summary>
	public partial class DatabaseView : UserControl
	{
		public DatabaseView()
		{
			InitializeComponent();
			
			DataContext = this;
			
			comboWhere.ItemsSource = DatabaseViewModelProvider.SupportedWhereClauses;
			comboOrderBy.ItemsSource = DatabaseViewModelProvider.SupportedOrderingClauses;

            tokenizerPeople.TokenMatcher = TokenMatcher;
            tokenizerLocation.TokenMatcher = TokenMatcher;
            tokenizerEvent.TokenMatcher = TokenMatcher;
            tokenizerKeywords.TokenMatcher = TokenMatcher;
        }

        private static String TokenMatcher(String text)
        {
            if (text.EndsWith(";") || text.EndsWith(","))
                return text.Substring(0, text.Length - 1).Trim().ToUpper();
            return null;
        }

		public void ReloadFiles()
		{
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var provider = new DatabaseViewModelProvider(
                    comboWhere.SelectedItem as String,
                    comboOrderBy.SelectedItem as String,
                    fromDate.SelectedDate, toDate.SelectedDate);
                var items = new AsyncVirtualizingCollection<FileInformationViewModel>(provider, 40, 10 * 1000);
                listControl.DataContext = items;
            }
		}
		
		private void UserControl_GotFocus(object sender, RoutedEventArgs e)
		{
			ReloadFiles();
		}

        private void RefreshMetadataAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var fi in Engine.Funani.FileInformation)
                Funani.Gui.Engine.Funani.RefreshMetadata(fi);
            ReloadFiles();
        }

        private void RefreshMetadata_Click(object sender, RoutedEventArgs e)
		{
			var canvas = sender as Control;
			var viewModel = canvas.DataContext as FileInformationViewModel;
			if (listControl.SelectedItem == null || !listControl.SelectedItems.Contains(viewModel))
			{
				viewModel.RefreshMetadata();
			}
			else
			{
				foreach (FileInformationViewModel item in listControl.SelectedItems)
				{
					viewModel.RefreshMetadata();
				}
			}
		}
		
		private void ReloadFiles_Handler(object sender, SelectionChangedEventArgs e)
		{
			ReloadFiles();
		}

	}
}