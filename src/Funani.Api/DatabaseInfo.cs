using System;
using Catel.Data;

namespace Funani.Api
{
    public class DatabaseInfo : SavableModelBase<DatabaseInfo>
    {
        public DatabaseInfo()
        {
            Guid = Guid.NewGuid();
        }

        #region Property: Guid
        /// <summary>
        /// Guid.
        /// </summary>
        public Guid Guid
        {
            get;
            private set;
        }

        #endregion

        #region Property: Title
        /// <summary>
        /// Title.
        /// </summary>
        public String Title
        {
            get;
            set;
        }

        #endregion

        #region Property: Description
        /// <summary>
        /// Description.
        /// </summary>
        public String Description
        {
            get;
            set;
        }

        #endregion
    }
}