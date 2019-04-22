
using System;
using Catel.MVVM;
using Funani.Api;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     FunaniDatabase view model.
    /// </summary>
    public class FunaniDatabaseViewModel : ViewModelBase
    {
        #region Fields

        private readonly IEngine _engine;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunaniDatabaseViewModel" /> class.
        /// </summary>
        public FunaniDatabaseViewModel(IEngine engine)
        {
            _engine = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Funani Database View model"; }
        }

        public DatabaseInfo DatabaseInfo
        {
            get { return _engine.DatabaseInfo; }
        }

        public String DatabasePath
        {
            get { return _engine.DatabasePath; }
        }

        public long TotalFileCount
        {
            get { return _engine.TotalFileCount; }
        }

        #endregion

        #region Commands

        #endregion

        #region Methods

        #endregion
    }
}