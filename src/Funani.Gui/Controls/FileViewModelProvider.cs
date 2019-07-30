
using System;
using System.Collections.Generic;
using System.Linq;

using Funani.Gui.Model;
using Funani.Gui.ViewModels;

namespace Funani.Gui.Controls
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileViewModelProvider" /> class.
        /// </summary>
        public FileViewModelProvider(String path, bool filterAlreadyStored)
        {
            _di = new DirectoryInfo(path);
            _filterAlreadyStored = filterAlreadyStored;
            try
            {
                _files = _di.EnumerateFiles();
            }
            catch (Exception)
            {
                // TODO: handle probable access rights issue in a cleaner fashion
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
                _filteredFiles = _files.Select(x => new FileViewModel(x))
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
            if (_filterAlreadyStored)
            {
                return _filteredFiles
                    .Skip(startIndex)
                    .Take(count)
                    .ToList();
            }
            else
            {
                return _files
                    .Skip(startIndex)
                    .Take(count)
                    .Select(x => new FileViewModel(x))
                    .ToList();
            }
        }
    }
}