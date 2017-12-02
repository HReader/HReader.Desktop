using Caliburn.Micro;
using HReader.Base;

namespace HReader.ViewModels
{
    internal class MetadataSourceViewModel : Screen
    {
        public MetadataSourceViewModel(IMetadataSource source)
        {
            this.source = source;
        }

        private readonly IMetadataSource source;

        public string Name => source.Name;
        public string Author => source.Author;
        public string Version => source.Version;
    }
}
