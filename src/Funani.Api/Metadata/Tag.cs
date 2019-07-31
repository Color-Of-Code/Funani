namespace Funani.Api.Metadata
{
    public class Tag
    {
        public string Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TagType Type { get; set; }
    }
}