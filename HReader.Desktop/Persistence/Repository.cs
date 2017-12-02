using Newtonsoft.Json;
using PropertyChanged;

namespace HReader.Persistence
{
    internal enum RepositoryKind
    {
        Unknown = 0,
        Directory = 1,
        Online = 2,
    }
    
    [AddINotifyPropertyChangedInterface]
    [JsonObject(MemberSerialization.OptIn)]
    internal class Repository
    {
        [JsonProperty("name")]
        public string Name      { get; set; } = string.Empty;
        [JsonProperty("path")]
        public string Path { get; set; } = string.Empty;
        [JsonProperty("kind")]
        public RepositoryKind Kind { get; set; } = RepositoryKind.Unknown;
    }
}
