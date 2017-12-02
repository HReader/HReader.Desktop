using Newtonsoft.Json;

namespace HReader.Persistence
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ApplicationState
    {
        [JsonProperty("active_tab")]
        public string ActiveTab { get; set; } = string.Empty;
    }
}
