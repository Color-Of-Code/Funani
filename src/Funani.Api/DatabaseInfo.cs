namespace Funani.Api
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    [Serializable]
    public class DatabaseInfo : SavableModelBase<DatabaseInfo>
    {
        public DatabaseInfo()
        {
            this.Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Guid.
        /// </summary>
        public Guid Guid
        {
            get;
            private set;
        }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        protected DatabaseInfo(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}