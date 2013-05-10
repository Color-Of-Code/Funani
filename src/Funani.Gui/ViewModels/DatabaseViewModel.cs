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

using Catel.MVVM;
using Funani.Api;
using Funani.Api.Metadata;
using Funani.Gui.Controls.FileExplorer;
using Funani.Gui.Model;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     Implementation of IItemsProvider returning <see cref="FileViewModel" /> items
    /// </summary>
    public class DatabaseViewModel : ViewModelBase, IItemsProvider<FileInformationViewModel>
    {
        private readonly Boolean _deleted;
        private readonly String _orderByClause;
        private readonly String _regexTitle;
        private readonly String _whereClause;
        private DateTime? _fromDate;
        private DateTime? _toDate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileViewModelProvider" /> class.
        /// </summary>
        public DatabaseViewModel(Boolean deleted, String regexTitle,
                                 String whereClause, String orderByClause, DateTime? fromDate, DateTime? toDate)
        {
            _engine = ServiceLocator.ResolveType<IEngine>();
            _deleted = deleted;
            _regexTitle = regexTitle;
            _fromDate = fromDate;
            _toDate = toDate;
            _whereClause = whereClause;
            _orderByClause = orderByClause;
        }

        private readonly IEngine _engine;

        public static IEnumerable<String> SupportedOrderingClauses
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

        public static IEnumerable<String> SupportedWhereClauses
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
            IQueryable<FileInformation> query = BuildQuery();
            return query
                .Skip(startIndex)
                .Take(count)
                .Select(x => new FileInformationViewModel(x, _engine))
                .ToList();
        }

        private IQueryable<FileInformation> BuildQuery()
        {
            IQueryable<FileInformation> query = _engine.FileInformation;
            if (_deleted)
                query = query.Where(x => x.IsDeleted);
            else
                query = query.Where(x => !x.IsDeleted);

            if (!String.IsNullOrWhiteSpace(_regexTitle))
            {
                try
                {
                    var regex = new Regex(_regexTitle);
                    query = query.Where(x => regex.IsMatch(x.Title));
                }
                catch
                {
                }
            }
            if (_fromDate.HasValue)
            {
                query = query.Where(x => x.DateTaken >= _fromDate.Value);
            }
            if (_toDate.HasValue)
            {
                DateTime toDate = _toDate.Value.AddDays(1);
                query = query.Where(x => x.DateTaken <= toDate);
            }

            if (_whereClause == "images")
                query = query.Where(x => x.MimeType.StartsWith("image/"));
            else if (_whereClause == "videos")
                query = query.Where(x => x.MimeType.StartsWith("video/"));
            else if (_whereClause == "others")
                query = query.Where(x => !x.MimeType.StartsWith("image/") && !x.MimeType.StartsWith("video/"));

            if (_orderByClause == "DateTaken descending")
                query = query.OrderByDescending(x => x.DateTaken);
            else if (_orderByClause == "DateTaken ascending")
                query = query.OrderBy(x => x.DateTaken);
            else if (_orderByClause == "Size descending")
                query = query.OrderByDescending(x => x.FileSize);
            else if (_orderByClause == "Size ascending")
                query = query.OrderBy(x => x.FileSize);
            else if (_orderByClause == "Rating descending")
                query = query.OrderByDescending(x => x.Rating);
            else if (_orderByClause == "Rating ascending")
                query = query.OrderBy(x => x.Rating);

            return query;
        }
    }
}