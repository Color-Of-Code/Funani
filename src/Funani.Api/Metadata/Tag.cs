

using System;

namespace Funani.Api.Metadata
{
    public class Tag
    {
        public String Id { get; private set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public TagType Type { get; set; }
    }
}