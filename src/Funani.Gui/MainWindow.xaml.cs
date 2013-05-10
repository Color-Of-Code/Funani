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
using System.IO;
using System.Windows;
using Funani.Api;
using Funani.Engine;
using Funani.Gui.Controls.Progress;
using Funani.Gui.Properties;
using Microsoft.Win32;

namespace Funani.Gui
{
    using SWF = System.Windows.Forms;
    using Catel.IoC;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandProgressViewModel _commandQueue;
        private readonly IEngine _engine;

        public MainWindow()
        {
            InitializeComponent();

            Settings.Default.Upgrade();

            _engine = ServiceLocator.Default.ResolveType<IEngine>();
            FunaniDatabase.DataContext = _engine;

            _commandQueue = new CommandProgressViewModel(_engine.CommandQueue);
            Progress.DataContext = _commandQueue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings settings = Settings.Default;
            EnsureMongodbPathIsValid();
            EnsureFunanidbPathIsValid();
            _engine.OpenDatabase(settings.MongodbPath, settings.LastFunaniDatabase, MongoDbView.MongoDbListener);
            if (!String.IsNullOrWhiteSpace(settings.LastDirectoryExplorerSelectedPath))
            {
                DirectoryExplorer.SelectPath(settings.LastDirectoryExplorerSelectedPath);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _engine.CloseDatabase();
            Settings settings = Settings.Default;
            settings.LastDirectoryExplorerSelectedPath = DirectoryExplorer.SelectedPath;
            settings.Save();
        }

        private void EnsureFunanidbPathIsValid()
        {
            String funanidbPath = Settings.Default.LastFunaniDatabase;
            if (!_engine.IsValidDatabase(funanidbPath))
            {
                var ofd = new SWF.FolderBrowserDialog();
                ofd.Description = "Browse to a valid Funani DB";
                ofd.ShowNewFolderButton = true;
                if (ofd.ShowDialog() == SWF.DialogResult.OK)
                {
                    var di = new DirectoryInfo(ofd.SelectedPath);
                    Settings settings = Settings.Default;
                    settings.LastFunaniDatabase = di.FullName;
                    settings.Save();
                    return;
                }

                Close();
            }
        }

        private static Boolean IsMongodbPathValid(String path)
        {
            if (String.IsNullOrWhiteSpace(path) ||
                !Directory.Exists(path) ||
                !File.Exists(Path.Combine(path, "mongod.exe")))
                return false;
            return true;
        }

        private void EnsureMongodbPathIsValid()
        {
            String mongodbPath = Settings.Default.MongodbPath;
            if (!IsMongodbPathValid(mongodbPath))
            {
                var ofd = new OpenFileDialog();
                ofd.Title = "Browse to the mongoDB executable";
                ofd.CheckFileExists = true;
                ofd.Filter = "Looking for mongod.exe|mongod.exe";
                if (ofd.ShowDialog() == true)
                {
                    var fi = new FileInfo(ofd.FileName);
                    if (IsMongodbPathValid(fi.DirectoryName))
                    {
                        Settings settings = Settings.Default;
                        settings.MongodbPath = fi.DirectoryName;
                        settings.Save();
                        return;
                    }
                }

                Close();
            }
        }
    }
}