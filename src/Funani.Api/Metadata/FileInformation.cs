namespace Funani.Api.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using Catel.IoC;
    using Funani.Core.Hash;

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
    public class FileInformation
    {
        private readonly IEngine _engine;

        public FileInformation()
        {
            this.Paths = new List<string>();
            this._engine = ServiceLocator.Default.ResolveType<IEngine>();
        }

        public FileInformation(IFileInfo file)
            : this()
        {
            this.Id = new Algorithms(file.FileSystem).ComputeSha1(file);
            this.FileSize = file.Length;
            this.Title = file.Name;
            this.MimeType = MimeExtractor.MimeType.Extract(file);
            RefreshMetadata(file);
            this.AddPath(file);
        }

        /// <summary>
        ///     Gets or sets the Id value.
        /// </summary>
        public String Id { get; private set; }

        public Int64 FileSize { get; private set; }

        /// <summary>
        ///     Gets or sets the MimeType value.
        /// </summary>
        public string MimeType { get; private set; }

        public IList<string> Paths { get; private set; }

        public long Width { get; private set; }
        public long Height { get; private set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Title { get; set; }

        public DateTime? DateTaken { get; set; } // start date for video
        public Int64 Duration { get; set; } // for videos, sound

        public DateTime? LastModification { get; set; }

        public string Device { get; set; } // digitalizing device
        public string ApplicationName { get; set; } // application used to process the data

        // orientation for view
        public int? Angle { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 0 -> 5
        /// </summary>
        public int? Rating { get; set; }

        public IList<Tag> Tags { get; private set; }

        /// <summary>
        ///     Persist to the mongo db
        /// </summary>
        public void Save()
        {
            this._engine.Save(this);
        }

        public void AddPath(IFileInfo file)
        {
            if (!this.Paths.Contains(file.FullName))
            {
                this.Paths.Add(file.FullName);
            }
        }

        public void RefreshMetadata()
        {
            this._engine.RefreshMetadata(this);
        }

        public void RefreshMetadata(IFileInfo file)
        {
            var uri = new Uri(file.FullName);
            var metadata = MetadataExtractor.Metadata.Extract(uri, this.MimeType);
            if (metadata != null)
            {
                if (metadata.ContainsKey("Width"))
                {
                    this.Width = Convert.ToInt64(metadata["Width"]);
                }

                if (metadata.ContainsKey("Height"))
                {
                    this.Height = Convert.ToInt64(metadata["Height"]);
                }

                if (metadata.ContainsKey("Device"))
                {
                    this.Device = metadata["Device"];
                }

                if (metadata.ContainsKey("DateTaken"))
                {
                    this.DateTaken = DateTime.ParseExact(metadata["DateTaken"], "dd.MM.yyyy HH:mm:ss", null);
                }

                if (metadata.ContainsKey("ApplicationName"))
                {
                    this.ApplicationName = metadata["ApplicationName"];
                }

                if (metadata.ContainsKey("Angle"))
                {
                    this.Angle = int.Parse(metadata["Angle"]);
                }
            }
        }

        public void AddTag(Tag tag)
        {
            if (!this.Tags.Contains(tag))
            {
                this.Tags.Add(tag);
            }
        }
    }
}
