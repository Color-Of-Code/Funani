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
using System.ComponentModel;
using System.Windows.Threading;
using Funani.Api;
using MongoDB.Driver;

namespace Funani.Gui.Controls
{
    public class MongoDbViewModel : IConsoleRedirect, INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private readonly IEngine _engine;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void TriggerPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MongoDbViewModel(IEngine engine, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _engine = engine;
            Lines = new ObservableCollection<string>();
        }

        public ObservableCollection<String> Lines { get; private set; }

        public String Query { get; set; }

        public IList<String> QueryResults { get; set; }

        public DatabaseStatsResult Statistics
        {
            get
            {
                if (Funani == null)
                    return null;
                return Funani.GetStats();
            }
        }

        private MongoDatabase Funani
        {
            get { return _engine.MetadataDatabase as MongoDatabase; }
        }

        public void OnOutputDataReceived(string data)
        {
            if (data != null)
            {
                _dispatcher.BeginInvoke((Action) (() =>
                                                  Lines.Add(data.TrimEnd()))
                    );
            }
        }

        public void OnErrorDataReceived(string data)
        {
            if (data != null)
            {
                _dispatcher.BeginInvoke((Action) (() =>
                                                  Lines.Add(data.TrimEnd()))
                    );
            }
        }

        public void RunQuery()
        {
            TriggerPropertyChanged("QueryResults");
        }

        public void RefreshStatistics()
        {
            TriggerPropertyChanged("Statistics");
        }

        public void Backup()
        {
            _engine.Backup();
        }
    }
}