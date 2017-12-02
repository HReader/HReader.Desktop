using Caliburn.Micro;
using HReader.Base;

namespace HReader.ViewModels
{
    internal class ContentSourceViewModel : Screen
    {
        public ContentSourceViewModel(IContentSource source)
        {
            this.source = source;
        }

        private readonly IContentSource source;

        public string Name => source.Name;
        public string Author => source.Author;
        public string Version => source.Version;
    }
}
