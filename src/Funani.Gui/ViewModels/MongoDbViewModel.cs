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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Catel.Data;
using Catel.MVVM;

using Funani.Api;

using MongoDB.Driver;

namespace Funani.Gui.ViewModels
{
    public class MongoDbViewModel : ViewModelBase, IConsoleRedirect
    {
        private readonly Dispatcher _dispatcher;
        private readonly IEngine _engine;

        public MongoDbViewModel(Dispatcher dispatcher)
        {
            _engine = GetService<IEngine>();
            _dispatcher = dispatcher;
            Lines = new ObservableCollection<string>();

            // commands
            GetStatistics = new Command(OnGetStatisticsExecute);
            Backup = new Command(OnBackupExecute);
            RunQuery = new Command(OnRunQueryExecute);
        }

        public ObservableCollection<String> Lines { get; private set; }

        public String Query { get; set; }

        public IList<String> QueryResults { get; set; }

        /// <summary>
        /// Retrieves the statistics.
        /// </summary>
        public DatabaseStatsResult Statistics
        {
            get { return GetValue<DatabaseStatsResult>(StatisticsProperty); }
            set { SetValue(StatisticsProperty, value); }
        }

        /// <summary>
        /// Register the Statistics property so it is known in the class.
        /// </summary>
        public static readonly PropertyData StatisticsProperty = RegisterProperty("Statistics", typeof(DatabaseStatsResult), null);

        private MongoDatabase Funani
        {
            get { return _engine.MetadataDatabase as MongoDatabase; }
        }

        public void OnOutputDataReceived(string data)
        {
            if (data != null)
            {
                _dispatcher.BeginInvoke((Action)(() =>
                                                  Lines.Add(data.TrimEnd()))
                    );
            }
        }

        public void OnErrorDataReceived(string data)
        {
            if (data != null)
            {
                _dispatcher.BeginInvoke((Action)(() =>
                                                  Lines.Add(data.TrimEnd()))
                    );
            }
        }

        #region Command: GetStatistics
        /// <summary>
        /// Gets the GetStatistics command.
        /// </summary>
        public Command GetStatistics { get; private set; }

        /// <summary>
        /// Method to invoke when the GetStatistics command is executed.
        /// </summary>
        private void OnGetStatisticsExecute()
        {
            Statistics = Funani.GetStats();
        }
        #endregion

        #region Command: Backup
        /// <summary>
        /// Gets the Backup command.
        /// </summary>
        public Command Backup { get; private set; }

        /// <summary>
        /// Method to invoke when the Backup command is executed.
        /// </summary>
        private void OnBackupExecute()
        {
            _engine.Backup();
        }
        #endregion

        #region Command: RunQuery
        /// <summary>
        /// Gets the RunQuery command.
        /// </summary>
        public Command RunQuery { get; private set; }

        /// <summary>
        /// Method to invoke when the RunQuery command is executed.
        /// </summary>
        private void OnRunQueryExecute()
        {
            RaisePropertyChanged("QueryResults");
        }
        #endregion

    }
}