namespace Funani.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    public class DatabaseInfo : IXmlSerializable
    {
        public DatabaseInfo()
        {
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; private set; }
        public String Title { get; set; }
        public String Description { get; set; }

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            Guid = Guid.Parse(reader.ReadElementString("Guid"));
            Title = reader.ReadElementString("Title");
            Description = reader.ReadElementString("Description");
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Guid", Guid.ToString());
            writer.WriteElementString("Title", Title);
            writer.WriteElementString("Description", Description);
        }

        #endregion
    }
}
