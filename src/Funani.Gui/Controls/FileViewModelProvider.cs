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
    /// Demo implementation of IItemsProvider returning <see cref="FileViewModel"/> items after
    /// a pause to simulate network/disk latency.
    /// </summary>
    public class FileViewModelProvider : IItemsProvider<FileViewModel>
    {
        private readonly int _fetchDelay;
        private readonly DirectoryInfo _di;
        private IEnumerable<FileInfo> _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileViewModelProvider"/> class.
        /// </summary>
        public FileViewModelProvider(String path)
        {
            _di = new DirectoryInfo(path);
            _files = _di.EnumerateFiles();
            _fetchDelay = 0;
        }

        /// <summary>
        /// Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            return _files.Count();
        }

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<FileViewModel> FetchRange(int startIndex, int count)
        {
            if (_fetchDelay > 0)
                Thread.Sleep(_fetchDelay);

            List<FileViewModel> list = new List<FileViewModel>();
            list.AddRange(_files.Skip(startIndex).Take(count).Select(x => new FileViewModel(x)));
            return list;
        }
    }
}
