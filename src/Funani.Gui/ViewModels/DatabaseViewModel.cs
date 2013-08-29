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
using System.Linq;
using System.Text.RegularExpressions;
using Catel.Data;
using Catel.Logging;
using Catel.MVVM;
using Funani.Api;
using Funani.Api.Metadata;
using Funani.Gui.Model;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     Implementation of IItemsProvider returning <see cref="FileInformationViewModel" /> items
    /// </summary>
    public class DatabaseViewModel : ViewModelBase, IItemsProvider<FileInformationViewModel>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Register the FileInformationViewModels property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FileInformationViewModelsProperty =
            RegisterProperty("FileInformationViewModels", typeof(IEnumerable<FileInformationViewModel>), null);

        private readonly IEngine _engine;

        public DatabaseViewModel(IEngine engine)
        {
            _engine = engine;

            Refresh = new Command(OnRefreshExecute);
            RefreshAllMetadata = new Command(OnRefreshAllMetadataExecute);
            RefreshSelectedMetadata = new Command(OnRefreshSelectedMetadataExecute);
        }

        #region Property: WhereClause

        /// <summary>
        ///     Register the WhereClause property so it is known in the class.
        /// </summary>
        public static readonly PropertyData WhereClauseProperty = RegisterProperty("WhereClause", typeof(String),
                                                                                   null,
                                                                                   (sender, e) =>
                                                                                   ((DatabaseViewModel)sender)
                                                                                       .OnWhereClauseChanged());

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public String WhereClause
        {
            get { return GetValue<String>(WhereClauseProperty); }
            set { SetValue(WhereClauseProperty, value); }
        }

        /// <summary>
        ///     Called when the WhereClause property has changed.
        /// </summary>
        private void OnWhereClauseChanged()
        {
            OnRefreshExecute();
        }

        #endregion

        #region Property: OrderByClause

        /// <summary>
        ///     Register the OrderByClause property so it is known in the class.
        /// </summary>
        public static readonly PropertyData OrderByClauseProperty = RegisterProperty("OrderByClause", typeof(String),
                                                                                     null,
                                                                                     (sender, e) =>
                                                                                     ((DatabaseViewModel)sender)
                                                                                         .OnOrderByClauseChanged());

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public String OrderByClause
        {
            get { return GetValue<String>(OrderByClauseProperty); }
            set { SetValue(OrderByClauseProperty, value); }
        }

        /// <summary>
        ///     Called when the OrderByClause property has changed.
        /// </summary>
        private void OnOrderByClauseChanged()
        {
            OnRefreshExecute();
        }

        #endregion

        #region Property: RegularExpression

        /// <summary>
        ///     Register the RegularExpression property so it is known in the class.
        /// </summary>
        public static readonly PropertyData RegularExpressionProperty =
            RegisterProperty("RegularExpression", typeof(String), null,
                             (sender, e) => ((DatabaseViewModel)sender).OnRegularExpressionChanged());

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public String RegularExpression
        {
            get { return GetValue<String>(RegularExpressionProperty); }
            set { SetValue(RegularExpressionProperty, value); }
        }

        /// <summary>
        ///     Called when the RegularExpression property has changed.
        /// </summary>
        private void OnRegularExpressionChanged()
        {
            OnRefreshExecute();
        }

        #endregion

        #region Property: QueryDeletedFiles

        /// <summary>
        ///     Register the QueryDeletedFiles property so it is known in the class.
        /// </summary>
        public static readonly PropertyData QueryDeletedFilesProperty = RegisterProperty("QueryDeletedFiles",
                                                                                         typeof(Boolean), false,
                                                                                         (sender, e) =>
                                                                                         ((DatabaseViewModel)sender)
                                                                                             .OnQueryDeletedFilesChanged
                                                                                             ());

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public Boolean QueryDeletedFiles
        {
            get { return GetValue<Boolean>(QueryDeletedFilesProperty); }
            set { SetValue(QueryDeletedFilesProperty, value); }
        }

        /// <summary>
        ///     Called when the QueryDeletedFiles property has changed.
        /// </summary>
        private void OnQueryDeletedFilesChanged()
        {
            OnRefreshExecute();
        }

        #endregion

        #region Property: StartDate

        /// <summary>
        ///     Register the StartDate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData StartDateProperty = RegisterProperty("StartDate",
                                                                                 typeof(DateTime?), null,
                                                                                 (sender, e) =>
                                                                                 ((DatabaseViewModel)sender)
                                                                                     .OnStartDateChanged());

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public DateTime? StartDate
        {
            get { return GetValue<DateTime?>(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        /// <summary>
        ///     Called when the StartDate property has changed.
        /// </summary>
        private void OnStartDateChanged()
        {
            OnRefreshExecute();
        }

        #endregion

        #region Property: EndDate

        /// <summary>
        ///     Register the EndDate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData EndDateProperty = RegisterProperty("EndDate",
                                                                               typeof(DateTime?), null,
                                                                               (sender, e) =>
                                                                               ((DatabaseViewModel)sender)
                                                                                   .OnEndDateChanged());

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public DateTime? EndDate
        {
            get { return GetValue<DateTime?>(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        /// <summary>
        ///     Called when the EndDate property has changed.
        /// </summary>
        private void OnEndDateChanged()
        {
            OnRefreshExecute();
        }

        #endregion

        /// <summary>
        ///     Gets or sets the property value.
        /// </summary>
        public IEnumerable<FileInformationViewModel> FileInformationViewModels
        {
            get { return GetValue<IEnumerable<FileInformationViewModel>>(FileInformationViewModelsProperty); }
            private set { SetValue(FileInformationViewModelsProperty, value); }
        }

        public IEnumerable<String> SupportedOrderByClauses
        {
            get
            {
                return new[]
                    {
                        "Default",
                        "DateTaken descending",
                        "DateTaken ascending",
                        "Size descending",
                        "Size ascending",
                        "Rating descending",
                        "Rating ascending"
                    };
            }
        }

        public IEnumerable<String> SupportedWhereClauses
        {
            get
            {
                return new[]
                    {
                        "All",
                        "images",
                        "videos",
                        "others"
                    };
            }
        }

        /// <summary>
        ///     Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            return BuildQuery().Count();
        }

        /// <summary>
        ///     Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<FileInformationViewModel> FetchRange(int startIndex, int count)
        {
            try
            {
                IQueryable<FileInformation> query = BuildQuery();
                return query
                    .Skip(startIndex)
                    .Take(count)
                    .Select(x => new FileInformationViewModel(x))
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        #region Helpers

        private IQueryable<FileInformation> BuildQuery()
        {
            IQueryable<FileInformation> query = _engine.FileInformation;
            if (QueryDeletedFiles)
                query = query.Where(x => x.IsDeleted);
            else
                query = query.Where(x => !x.IsDeleted);

            if (!String.IsNullOrWhiteSpace(RegularExpression))
            {
                try
                {
                    var regex = new Regex(RegularExpression, RegexOptions.IgnoreCase);
                    query = query.Where(x => regex.IsMatch(x.Title));
                }
                catch
                {
                }
            }
            if (StartDate.HasValue)
            {
                query = query.Where(x => x.DateTaken >= StartDate.Value);
            }
            if (EndDate.HasValue)
            {
                DateTime toDate = EndDate.Value.AddDays(1);
                query = query.Where(x => x.DateTaken <= toDate);
            }

            string whereClause = WhereClause;
            if (whereClause == "images")
                query = query.Where(x => x.MimeType.StartsWith("image/"));
            else if (whereClause == "videos")
                query = query.Where(x => x.MimeType.StartsWith("video/"));
            else if (whereClause == "others")
                query = query.Where(x => !x.MimeType.StartsWith("image/") && !x.MimeType.StartsWith("video/"));

            string orderByClause = OrderByClause;
            if (orderByClause == "DateTaken descending")
                query = query.OrderByDescending(x => x.DateTaken);
            else if (orderByClause == "DateTaken ascending")
                query = query.OrderBy(x => x.DateTaken);
            else if (orderByClause == "Size descending")
                query = query.OrderByDescending(x => x.FileSize);
            else if (orderByClause == "Size ascending")
                query = query.OrderBy(x => x.FileSize);
            else if (orderByClause == "Rating descending")
                query = query.OrderByDescending(x => x.Rating);
            else if (orderByClause == "Rating ascending")
                query = query.OrderBy(x => x.Rating);

            return query;
        }

        #endregion

        #region Command: Refresh

        /// <summary>
        ///     Gets the Refresh command.
        /// </summary>
        public Command Refresh { get; private set; }

        /// <summary>
        ///     Method to invoke when the Refresh command is executed.
        /// </summary>
        private void OnRefreshExecute()
        {
            FileInformationViewModels = new AsyncVirtualizingCollection<FileInformationViewModel>(this, 40, 10 * 1000);
        }

        #endregion

        #region Command: RefreshAllMetadata

        /// <summary>
        ///     Gets the RefreshAllMetadata command.
        /// </summary>
        public Command RefreshAllMetadata { get; private set; }

        /// <summary>
        ///     Refresh Metadata for each entry in the database
        /// </summary>
        private void OnRefreshAllMetadataExecute()
        {
            IEngine engine = _engine;
            foreach (FileInformation fi in engine.FileInformation)
                engine.RefreshMetadata(fi);
            OnRefreshExecute();
        }

        #endregion

        #region Command: RefreshSelectedMetadata

        /// <summary>
        ///     Gets the RefreshSelectedMetadata command.
        /// </summary>
        public Command RefreshSelectedMetadata { get; private set; }

        /// <summary>
        ///     Refresh Metadata for the selected entries in the database
        /// </summary>
        private void OnRefreshSelectedMetadataExecute()
        {
            throw new NotImplementedException();
            IEngine engine = _engine;
            foreach (FileInformation fi in engine.FileInformation)
                engine.RefreshMetadata(fi);
            OnRefreshExecute();
            ////if (ListControl.SelectedItem == null || !ListControl.SelectedItems.Contains(viewModel))
            ////{
            ////    viewModel.RefreshMetadata();
            ////}
            ////else
            ////{
            ////    foreach (FileInformationViewModel item in ListControl.SelectedItems)
            ////    {
            ////        item.RefreshMetadata();
            ////    }
            ////}
        }

        #endregion

        /// <summary>
        ///     Tokenizer used inside the tagging controls
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static String TokenMatcher(String text)
        {
            if (text.EndsWith(";") || text.EndsWith(","))
                return text.Substring(0, text.Length - 1).Trim().ToUpper();
            return null;
        }
    }
}