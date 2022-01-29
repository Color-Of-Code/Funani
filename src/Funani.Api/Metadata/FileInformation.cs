namespace Funani.Api.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
        private readonly IEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInformation"/> class.
        /// </summary>
        public FileInformation()
        {
            this.Paths = new List<string>();
            this.engine = ServiceLocator.Default.ResolveType<IEngine>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInformation"/> class.
        /// </summary>
        /// <param name="file"></param>
        public FileInformation(IFileInfo file)
            : this()
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.Id = new Algorithms(file.FileSystem).ComputeSha1(file);
            this.FileSize = file.Length;
            this.Title = file.Name;
            this.MimeType = MimeExtractor.MimeType.Extract(file);
            this.RefreshMetadata(file);
            this.AddPath(file);
        }

        public string Id { get; private set; }

        public long FileSize { get; private set; }

        public string MimeType { get; private set; }

        public IList<string> Paths { get; private set; }

        public long Width { get; private set; }

        public long Height { get; private set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Title { get; set; }

        public DateTime? DateTaken { get; set; } // start date for video

        public long Duration { get; set; } // for videos, sound

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
            this.engine.Save(this);
        }

        public void AddPath(IFileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!this.Paths.Contains(file.FullName))
            {
                this.Paths.Add(file.FullName);
            }
        }

        public void RefreshMetadata()
        {
            this.engine.RefreshMetadata(this);
        }

        public void RefreshMetadata(IFileInfo file)
        {
            var uri = new Uri(file.FullName);
            var metadata = MetadataExtractor.Metadata.Extract(uri, this.MimeType);
            if (metadata != null)
            {
                if (metadata.ContainsKey("Width"))
                {
                    this.Width = Convert.ToInt64(metadata["Width"], CultureInfo.InvariantCulture);
                }

                if (metadata.ContainsKey("Height"))
                {
                    this.Height = Convert.ToInt64(metadata["Height"], CultureInfo.InvariantCulture);
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
                    this.Angle = int.Parse(metadata["Angle"], CultureInfo.InvariantCulture);
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
