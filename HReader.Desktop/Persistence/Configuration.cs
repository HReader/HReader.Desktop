using System.Collections.Generic;
using System.IO;
using System.Text;
using HReader.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PropertyChanged;

namespace HReader.Persistence
{
    [AddINotifyPropertyChangedInterface]
    [JsonObject(MemberSerialization.OptIn)]
    internal class Configuration
    {
        private static JsonSerializer Serializer
            => JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(false),
                    new ShortcutBindingConverter(),
                },
            });

        private static string FileName => Path.Combine(Directories.Executable, "configuration.json");

        public static Configuration Load()
        {
            if (File.Exists(FileName))
            {
                // do not catch exception here, errors will propagate to the user since
                // they might have changed the configurtion file to an invalid state
                using (var reader = new JsonTextReader(new StreamReader(FileName, Encoding.UTF8)))
                {
                    return Serializer.Deserialize<Configuration>(reader);
                }
            }

            // create default configuration
            var config = new Configuration();
            config.Save();
            return config;
        }

        [JsonProperty("sources")]
        public Sources Sources { get; set; } = new Sources();

        [JsonProperty("preferences")]
        public UserPreferences UserPreferences { get; set; } = new UserPreferences();

        [JsonProperty("state")]
        public ApplicationState State { get; set; } = new ApplicationState();

        public void Save()
        {
            using (var writer = new JsonTextWriter(new StreamWriter(FileName, false, Encoding.UTF8)))
            {
                Serializer.Serialize(writer, this);
            }
        }
    }
}
