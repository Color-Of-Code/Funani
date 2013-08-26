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
using System.Linq;
using System.Windows.Threading;

using Catel.Data;
using Catel.IoC;
using Catel.MVVM;

using Funani.Api;

using MongoDB.Bson;
using MongoDB.Driver;
using OxyPlot;
using OxyPlot.Series;
using Funani.Api.Metadata;

namespace Funani.Gui.ViewModels
{
    public class MongoDbViewModel : ViewModelBase, IConsoleRedirect
    {
        private readonly Dispatcher _dispatcher;
        private readonly IEngine _engine;

        public MongoDbViewModel(Dispatcher dispatcher)
        {
            _engine = ServiceLocator.ResolveType<IEngine>();
            _dispatcher = dispatcher;

            // commands
            GetStatistics = new Command(OnGetStatisticsExecute);
            Backup = new Command(OnBackupExecute);
            RunQuery = new Command(OnRunQueryExecute);
        }

        private void LoadData()
        {
            var groupByMimeTypeOperation = new BsonDocument
            {
                {
                    "$group",
                    new BsonDocument
                    {
                        { "_id", "$MimeType" },
                        {
                            "SumCount",
                            new BsonDocument { { "$sum", 1 } }
                        },
                        {
                            "SumSize",
                            new BsonDocument { { "$sum", "$FileSize" } }
                        }
                    }
                }
            };

            var sortOperation = new BsonDocument { { "$sort", new BsonDocument { { "SumSize", -1 } } } };

            MongoDatabase db = _engine.MetadataDatabase as MongoDatabase;
            AggregateResult result = db.GetCollection<FileInformation>("fileinfo").Aggregate(
                groupByMimeTypeOperation,
                sortOperation);

            if (result.Ok)
            {
                var psCount = new PieSeries();
                var psSize = new PieSeries();

                var list = result.ResultDocuments
                	.Select(doc => new {
                	        	Mime = MimeMap(doc["_id"].AsString),
                	        	Size = doc["SumSize"].AsInt64 / 1024.0 / 1024.0 / 1024.0,
                	        	Count = doc["SumCount"].AsInt32
                	        });

                foreach (var doc in list.GroupBy(x => x.Mime)
                         .Select(doc => new {
                                 	Mime = doc.Key,
                                 	Size = doc.Sum(x => x.Size),
                                 	Count = doc.Sum(x => x.Count)
                                 })
                         .OrderByDescending(x => x.Size))
                {
                	var size = doc.Size;
                	var count = doc.Count;
                    psSize.Slices.Add(
                        new PieSlice()
                        {
                            Label = String.Format("{0:0.0} GB: {1}", size, doc.Mime),
                            Value = size
                        }
                        );
                    psCount.Slices.Add(
                        new PieSlice()
                        {
                            Label = String.Format("{0}: {1}", count, doc.Mime),
                            Value = count
                        }
                        );
                }

                psCount.InnerDiameter = 0.2;
                psCount.ExplodedDistance = 0;
                psCount.Stroke = OxyColors.Black;
                psCount.StrokeThickness = 1.0;
                psCount.AngleSpan = 360;
                psCount.StartAngle = 0;

                psSize.InnerDiameter = 0.2;
                psSize.ExplodedDistance = 0;
                psSize.Stroke = OxyColors.Black;
                psSize.StrokeThickness = 1.0;
                psSize.AngleSpan = 360;
                psSize.StartAngle = 0;

                var modelCount = new PlotModel("Count by MIME type");
                var modelSize = new PlotModel("Size by MIME type");

                modelCount.Series.Add(psCount);
                modelSize.Series.Add(psSize);

                MimeSizePlotModel = modelSize;
                MimeCountPlotModel = modelCount;
            }
        }
        
        private String MimeMap(String input)
        {
        	if (input.StartsWith("image/"))
        		return "images";
        	if (input.StartsWith("video/"))
        		return "videos";
        	if (input=="application/pdf")
        		return "documents";
        	return input;
        }

        #region Property: Lines

        public static readonly PropertyData LinesProperty =
            RegisterProperty("Lines", typeof(ObservableCollection<String>), new ObservableCollection<string>());

        public ObservableCollection<String> Lines
        {
            get { return GetValue<ObservableCollection<String>>(LinesProperty); }
            private set { SetValue(LinesProperty, value); }
        }

        #endregion

        #region Property: Query

        public static readonly PropertyData QueryProperty =
            RegisterProperty("Query", typeof(String), null);

        public String Query
        {
            get { return GetValue<String>(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        #endregion

        #region Property: QueryResults

        public static readonly PropertyData QueryResultsProperty =
            RegisterProperty("QueryResults", typeof(IList<String>), null);

        public IList<String> QueryResults
        {
            get { return GetValue<IList<String>>(QueryResultsProperty); }
            set { SetValue(QueryResultsProperty, value); }
        }

        #endregion

        #region Property: QueryException

        public static readonly PropertyData QueryExceptionProperty =
            RegisterProperty("QueryException", typeof(Exception), null);

        public Exception QueryException
        {
            get { return GetValue<Exception>(QueryExceptionProperty); }
            set { SetValue(QueryExceptionProperty, value); }
        }

        #endregion

        #region Property: CollectionNames
        public IEnumerable<String> CollectionNames
        {
            get { return GetValue<IEnumerable<String>>(CollectionNamesProperty); }
            set { SetValue(CollectionNamesProperty, value); }
        }

        public static readonly PropertyData CollectionNamesProperty =
            RegisterProperty("CollectionNames", typeof(IEnumerable<String>), null);
        #endregion

        #region Property: MimeCountPlotModel

        public static readonly PropertyData MimeCountPlotModelProperty =
            RegisterProperty("MimeCountPlotModel", typeof(PlotModel), null);

        public PlotModel MimeCountPlotModel
        {
            get { return GetValue<PlotModel>(MimeCountPlotModelProperty); }
            set { SetValue(MimeCountPlotModelProperty, value); }
        }

        #endregion

        #region Property: MimeSizePlotModel

        public static readonly PropertyData MimeSizePlotModelProperty =
            RegisterProperty("MimeSizePlotModel", typeof(PlotModel), null);

        public PlotModel MimeSizePlotModel
        {
            get { return GetValue<PlotModel>(MimeSizePlotModelProperty); }
            set { SetValue(MimeSizePlotModelProperty, value); }
        }

        #endregion

        #region Property: Statistics

        public static readonly PropertyData StatisticsProperty =
            RegisterProperty("Statistics", typeof(DatabaseStatsResult), null);

        public DatabaseStatsResult Statistics
        {
            get { return GetValue<DatabaseStatsResult>(StatisticsProperty); }
            set { SetValue(StatisticsProperty, value); }
        }

        #endregion

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

        #region Commands

        #region Command: GetStatistics

        /// <summary>
        ///     Gets the GetStatistics command.
        /// </summary>
        public Command GetStatistics { get; private set; }

        /// <summary>
        ///     Method to invoke when the GetStatistics command is executed.
        /// </summary>
        private void OnGetStatisticsExecute()
        {
            Statistics = Funani.GetStats();
            LoadData();
        }

        #endregion

        #region Command: Backup

        /// <summary>
        ///     Gets the Backup command.
        /// </summary>
        public Command Backup { get; private set; }

        /// <summary>
        ///     Method to invoke when the Backup command is executed.
        /// </summary>
        private void OnBackupExecute()
        {
            _engine.Backup();
        }

        #endregion

        #region Command: RunQuery

        /// <summary>
        ///     Gets the RunQuery command.
        /// </summary>
        public Command RunQuery { get; private set; }

        /// <summary>
        ///     Method to invoke when the RunQuery command is executed.
        /// </summary>
        private void OnRunQueryExecute()
        {
            if (CollectionNames == null)
                CollectionNames = Funani.GetCollectionNames();
            try
            {
                QueryException = null; // reset
                var bsonJS = new BsonJavaScript(Query);
                BsonValue result = Funani.Eval(bsonJS);
                QueryResults = result.ToJson().Split();
            }
            catch (Exception ex)
            {
                QueryException = ex;
            }
        }

        #endregion

        #endregion
    }
}