using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return "MainWindow"; } }

        #endregion

        #region Commands
        #endregion

        #region Methods
        #endregion
    }
}
