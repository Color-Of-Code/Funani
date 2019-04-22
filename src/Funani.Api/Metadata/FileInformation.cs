
using System;
using System.Collections.Generic;
using System.IO;
using Catel.Data;
using Catel.IoC;
using Funani.Api.Utils;

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
            Paths = new List<String>();
            _engine = ServiceLocator.Default.ResolveType<IEngine>();
        }

        public FileInformation(FileInfo file)
            : this()
        {
            Id = ComputeHash.SHA1(file);
            FileSize = file.Length;
            Title = file.Name;
            MimeType = MimeExtractor.MimeType.Extract(file);
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
            get { return _title; }
            set
            {
                _title = value;
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
            get { return _angle; }
            set
            {
                _angle = value;
                RaisePropertyChanged("Angle");
            }
        }

        public Boolean IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                _isDeleted = value;
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

        public void AddPath(FileInfo file)
        {
            if (!Paths.Contains(file.FullName))
                Paths.Add(file.FullName);
        }

        public void RefreshMetadata()
        {
            _engine.RefreshMetadata(this);
            RaisePropertyChanged(string.Empty);
        }

        public void RefreshMetadata(FileInfo file)
        {
            var uri = new Uri(file.FullName);
            Dictionary<string, string> metadata = MetadataExtractor.Metadata.Extract(uri, MimeType);
            if (metadata != null)
            {
                if (metadata.ContainsKey("Width"))
                    Width = Convert.ToInt64(metadata["Width"]);
                if (metadata.ContainsKey("Height"))
                    Height = Convert.ToInt64(metadata["Height"]);

                if (metadata.ContainsKey("Device"))
                    Device = metadata["Device"];
                if (metadata.ContainsKey("DateTaken"))
                    DateTaken = DateTime.ParseExact(metadata["DateTaken"], "dd.MM.yyyy HH:mm:ss", null);
                if (metadata.ContainsKey("ApplicationName"))
                    ApplicationName = metadata["ApplicationName"];
                if (metadata.ContainsKey("Angle"))
                    Angle = int.Parse(metadata["Angle"]);
            }
        }

        public void AddTag(Tag tag)
        {
            if (!Tags.Contains(tag))
                Tags.Add(tag);
        }
    }
}