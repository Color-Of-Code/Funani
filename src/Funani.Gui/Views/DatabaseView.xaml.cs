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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using Catel.IoC;

using Funani.Api;
using Funani.Api.Metadata;
using Funani.Gui.Model;
using Funani.Gui.ViewModels;

namespace Funani.Gui.Views
{
    /// <summary>
    ///     Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : Catel.Windows.Controls.UserControl
    {
        public DatabaseView()
        {
            InitializeComponent();

            DataContext = this;

            ComboWhere.ItemsSource = DatabaseViewModel.SupportedWhereClauses;
            ComboOrderBy.ItemsSource = DatabaseViewModel.SupportedOrderingClauses;

            TokenizerPeople.TokenMatcher = TokenMatcher;
            TokenizerLocation.TokenMatcher = TokenMatcher;
            TokenizerEvent.TokenMatcher = TokenMatcher;
            TokenizerKeywords.TokenMatcher = TokenMatcher;
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
                var provider = new DatabaseViewModel(
                    CheckBoxDeleted.IsChecked ?? false,
                    RegexLookFor.Text,
                    ComboWhere.SelectedItem as String,
                    ComboOrderBy.SelectedItem as String,
                    FromDate.SelectedDate, ToDate.SelectedDate);
                var items = new AsyncVirtualizingCollection<FileInformationViewModel>(provider, 40, 10 * 1000);
                ListControl.DataContext = items;
            }
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            ReloadFiles();
        }

        private void RefreshMetadataAll_Click(object sender, RoutedEventArgs e)
        {
            var engine = ServiceLocator.Default.ResolveType<IEngine>();
            foreach (FileInformation fi in engine.FileInformation)
                engine.RefreshMetadata(fi);
            ReloadFiles();
        }

        private void RefreshMetadata_Click(object sender, RoutedEventArgs e)
        {
            var canvas = sender as Control;
            Debug.Assert(canvas != null, "canvas is null");
            var viewModel = canvas.DataContext as FileInformationViewModel;
            Debug.Assert(viewModel != null, "viewModel != null");
            if (ListControl.SelectedItem == null || !ListControl.SelectedItems.Contains(viewModel))
            {
                viewModel.RefreshMetadata();
            }
            else
            {
                foreach (FileInformationViewModel item in ListControl.SelectedItems)
                {
                    item.RefreshMetadata();
                }
            }
        }

        private void ReloadFiles_Handler(object sender, RoutedEventArgs e)
        {
            ReloadFiles();
        }

        private void ReloadFiles_SelectionChanged_Handler(object sender, SelectionChangedEventArgs e)
        {
            ReloadFiles();
        }
    }
}