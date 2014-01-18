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
        ///     Gets or sets the property value.
        /// </summary>
        public String WhereClause
        {
            get;
            set;
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
        ///     Gets or sets the property value.
        /// </summary>
        public String OrderByClause
        {
            get;
            set;
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
        ///     Gets or sets the property value.
        /// </summary>
        public String RegularExpression
        {
            get;
            set;
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
        ///     Gets or sets the property value.
        /// </summary>
        public Boolean QueryDeletedFiles
        {
            get;
            set;
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
        ///     Gets or sets the property value.
        /// </summary>
        public DateTime? StartDate
        {
            get;
            set;
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
        ///     Gets or sets the property value.
        /// </summary>
        public DateTime? EndDate
        {
            get;
            set;
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
            get;
            private set;
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