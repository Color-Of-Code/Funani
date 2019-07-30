
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Catel.Data;
using Catel.IoC;
using Funani.Core.Hash;

namespace Funani.Api.Metadata
{
    /// <summary>
    ///     TODO: There is an issue with using Catel ModelBase together with MongoDB C# Driver
    ///     These attributes are created additionally and make trouble on reading back
    ///     "Mode" : 0,
    ///     "IsDirty" : true,
    ///     "IsReadOnly" : false,
    ///     "Validator" : null,
    ///     "ValidationContext" : {
    ///     "_t" : "ValidationContext"
    ///     },
    /// </summary>
    public class FileInformation : ObservableObject
    {
        private readonly IEngine _engine;
        private int? _angle;
        private bool _isDeleted;
        private int? _rating;
        private String _title;

        public FileInformation()
        {
            this.Paths = new List<String>();
            _engine = ServiceLocator.Default.ResolveType<IEngine>();
        }

        public FileInformation(IFileInfo file)
            : this()
        {
            this.Id = new Algorithms(file.FileSystem).ComputeSha1(file);
            this.FileSize = file.Length;
            this.Title = file.Name;
            this.MimeType = MimeExtractor.MimeType.Extract(file);
            RefreshMetadata(file);
            AddPath(file);
        }

        /// <summary>
        ///     Gets or sets the Id value.
        /// </summary>
        public String Id { get; private set; }

        public Int64 FileSize { get; private set; }

        /// <summary>
        ///     Gets or sets the MimeType value.
        /// </summary>
        public String MimeType { get; private set; }

        public IList<String> Paths { get; private set; }

        public Int64 Width { get; private set; }
        public Int64 Height { get; private set; }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public String Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                RaisePropertyChanged("Title");
            }
        }

        public DateTime? DateTaken { get; set; } // start date for video
        public Int64 Duration { get; set; } // for videos, sound

        public DateTime? LastModification { get; set; }

        public String Device { get; set; } // digitalizing device
        public String ApplicationName { get; set; } // application used to process the data

        // orientation for view
        public int? Angle
        {
            get { return this._angle; }
            set
            {
                this._angle = value;
                RaisePropertyChanged("Angle");
            }
        }

        public Boolean IsDeleted
        {
            get { return this._isDeleted; }
            set
            {
                this._isDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        /// <summary>
        /// 0 -> 5
        /// </summary>
        public int? Rating
        {
            get { return _rating; }
            set
            {
                _rating = value;
                RaisePropertyChanged("Rating");
            }
        }

        public IList<Tag> Tags { get; private set; }

        /// <summary>
        ///     Persist to the mongo db
        /// </summary>
        public void Save()
        {
            _engine.Save(this);
        }

        public void AddPath(IFileInfo file)
        {
            if (!Paths.Contains(file.FullName))
                Paths.Add(file.FullName);
        }

        public void RefreshMetadata()
        {
            _engine.RefreshMetadata(this);
            RaisePropertyChanged(string.Empty);
        }

        public void RefreshMetadata(IFileInfo file)
        {
            var uri = new Uri(file.FullName);
            Dictionary<string, string> metadata = MetadataExtractor.Metadata.Extract(uri, MimeType);
            if (metadata != null)
            {
                if (metadata.ContainsKey("Width"))
                    this.Width = Convert.ToInt64(metadata["Width"]);
                if (metadata.ContainsKey("Height"))
                    this.Height = Convert.ToInt64(metadata["Height"]);

                if (metadata.ContainsKey("Device"))
                    this.Device = metadata["Device"];
                if (metadata.ContainsKey("DateTaken"))
                    this.DateTaken = DateTime.ParseExact(metadata["DateTaken"], "dd.MM.yyyy HH:mm:ss", null);
                if (metadata.ContainsKey("ApplicationName"))
                    this.ApplicationName = metadata["ApplicationName"];
                if (metadata.ContainsKey("Angle"))
                    this.Angle = int.Parse(metadata["Angle"]);
            }
        }

        public void AddTag(Tag tag)
        {
            if (!Tags.Contains(tag))
                Tags.Add(tag);
        }
    }
}
