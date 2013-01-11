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
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;

	using Funani.Gui.Model;

	/// <summary>
	/// Implementation of IItemsProvider returning <see cref="FileViewModel"/> items
	/// </summary>
	public class DatabaseViewModelProvider : IItemsProvider<FileInformationViewModel>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileViewModelProvider"/> class.
		/// </summary>
		public DatabaseViewModelProvider(String whereClause, String orderByClause)
		{
			_whereClause = whereClause;
			_orderByClause = orderByClause;
		}

		public static IEnumerable<String> SupportedOrderingClauses
		{
			get
			{
				return new String[] {
					"Default",
					"DateTaken descending",
					"DateTaken ascending",
					"Size descending",
					"Size ascending"
				};
			}
		}
		
		public static IEnumerable<String> SupportedWhereClauses
		{
			get
			{
				return new String[] {
					"All",
					"images",
					"videos",
					"others"
				};
			}
		}

		/// <summary>
		/// Fetches the total number of items available.
		/// </summary>
		/// <returns></returns>
		public int FetchCount()
		{
			return BuildQuery().Count();
		}

		/// <summary>
		/// Fetches a range of items.
		/// </summary>
		/// <param name="startIndex">The start index.</param>
		/// <param name="count">The number of items to fetch.</param>
		/// <returns></returns>
		public IList<FileInformationViewModel> FetchRange(int startIndex, int count)
		{
			var query = BuildQuery();
			List<FileInformationViewModel> list = new List<FileInformationViewModel>();
			list.AddRange(
				query.Skip(startIndex).Take(count).Select(x => new FileInformationViewModel(x))
			);
			return list;
		}

		IQueryable<Funani.Api.Metadata.FileInformation> BuildQuery()
		{
			var query = Engine.Funani.FileInformation;

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

			return query;
		}
		
		private String _whereClause;
		private String _orderByClause;
	}
}
