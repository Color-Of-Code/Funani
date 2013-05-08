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
using System.IO;
using System.Linq;

using Funani.Api;
using Funani.Gui.Model;
using Funani.Gui.ViewModels;

namespace Funani.Gui.Controls.FileExplorer
{
    /// <summary>
    ///     Implementation of IItemsProvider returning <see cref="FileViewModel" /> items
    /// </summary>
    public class FileViewModelProvider : IItemsProvider<FileViewModel>
    {
        private readonly DirectoryInfo _di;
        private readonly IEnumerable<FileInfo> _files;
        private readonly bool _filterAlreadyStored;
        private IEnumerable<FileViewModel> _filteredFiles;
        private readonly IEngine _engine;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileViewModelProvider" /> class.
        /// </summary>
        public FileViewModelProvider(IEngine engine, String path, bool filterAlreadyStored)
        {
            _engine = engine;
            _di = new DirectoryInfo(path);
            _filterAlreadyStored = filterAlreadyStored;
            try
            {
                _files = _di.EnumerateFiles();
            }
            catch (Exception)
            {
                //TODO: handle probable access rights issue in a cleaner fashion
                _files = new List<FileInfo>(); // empty list...
            }
        }

        /// <summary>
        ///     Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            if (_filterAlreadyStored)
            {
                _filteredFiles = _files.Select(x => new FileViewModel(x, _engine))
                    .Where(x => x.IsStored == false);
                return _filteredFiles.Count();
            }
            return _files.Count();
        }

        /// <summary>
        ///     Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<FileViewModel> FetchRange(int startIndex, int count)
        {
            var list = new List<FileViewModel>();
            if (_filterAlreadyStored)
            {
                list.AddRange(_filteredFiles.Skip(startIndex).Take(count));
            }
            else
            {
                list.AddRange(_files.Skip(startIndex).Take(count).Select(x => new FileViewModel(x, _engine)));
            }
            return list;
        }
    }

}