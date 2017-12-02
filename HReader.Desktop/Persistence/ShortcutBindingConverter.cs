using System;
using HReader.Wpf.Shortcuts;
using Newtonsoft.Json;

namespace HReader.Persistence
{
    internal class ShortcutBindingConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type == typeof(ShortcutBinding);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = (string) reader.Value;
            return ShortcutBindingSerializer.Deserialize(token);
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(ShortcutBindingSerializer.Serialize((ShortcutBinding) value));
        }
    }
}