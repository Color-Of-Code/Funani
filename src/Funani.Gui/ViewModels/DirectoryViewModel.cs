
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Catel.MVVM;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     DirectoryViewModel is a recusirve collection of DirectoryViewModel's
    /// </summary>
    [DebuggerDisplay("{DirectoryInfo}")]
    public class DirectoryViewModel : ViewModelBase
    {
        private readonly Exception _exception;
        private readonly ObservableCollection<DirectoryViewModel> _items;
        private readonly DirectoryInfo _model;
        private readonly DirectoryViewModel _parent;

        private bool _isExpanded;
        private bool _isSelected;
        private bool _refresh;

        public DirectoryViewModel(DirectoryInfo model, DirectoryViewModel parent = null, Exception exception = null)
        {
            _model = model;
            _parent = parent;
            _exception = exception;
            _refresh = true;
            _items = new ObservableCollection<DirectoryViewModel>();
        }

        public String ExceptionText
        {
            get
            {
                if (_exception == null) return null;
                return _exception.Message;
            }
        }

        #region DirectoryViewModel Properties

        public ObservableCollection<DirectoryViewModel> Directories
        {
            get
            {
                if (_refresh)
                    Refresh();
                return _items;
            }
        }

        public String Name
        {
            get { return _model.Name; }
        }

        public String Icon
        {
            get
            {
                const string basePath = "pack://application:,,,/Images/";
                String icon = "folder";
                if (IsSelected)
                    icon = "folder-open";
                if (IsDrive)
                    icon = "drive-harddisk";
                return String.Format("{0}{1}.png", basePath, icon);
            }
        }

        public bool IsDrive
        {
            get { return _parent == null; }
        }

        public DirectoryInfo DirectoryInfo
        {
            get { return _model; }
        }

        #endregion

        #region Presentation Members

        #region IsExpanded

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                }

                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion

        #region IsSelected

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                    RaisePropertyChanged("Icon");
                }
            }
        }

        #endregion

        #region Parent

        public DirectoryViewModel Parent
        {
            get { return _parent; }
        }

        #endregion

        #endregion

        public DirectoryViewModel Lookup(DirectoryInfo path)
        {
            // if we found the path
            if (DirectoryInfo.FullName == path.FullName)
                return this;

            DirectoryInfo p = path;
            while (p.Parent.FullName != DirectoryInfo.FullName)
                p = p.Parent;

            DirectoryViewModel child = Directories.FirstOrDefault(x => x.DirectoryInfo.FullName == p.FullName);
            if (child == null)
                return null;
            return child.Lookup(path);
        }

        private void Refresh()
        {
            _items.Clear();
            foreach (DirectoryInfo d in _model.EnumerateDirectories())
            {
                if ((d.Attributes & FileAttributes.Hidden) != 0)
                    continue;
                try
                {
                    _items.Add(new DirectoryViewModel(d, this));
                }
                catch (Exception ex)
                {
                    _items.Add(new DirectoryViewModel(d, this, ex));
                }
            }
            _refresh = false;
        }
    }
}