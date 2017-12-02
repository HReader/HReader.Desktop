using System.Collections.Generic;
using HReader.Wpf.Shortcuts;
using Newtonsoft.Json;
using PropertyChanged;

namespace HReader.Persistence
{
    [AddINotifyPropertyChangedInterface]
    [JsonObject(MemberSerialization.OptIn)]
    internal class UserPreferences : IShortcutPersistenceProvider
    {
        [JsonProperty("shortcuts")]
        public IDictionary<string, IList<ShortcutBinding>> Shortcuts { get; set; } = new Dictionary<string, IList<ShortcutBinding>>();

        /// <inheritdoc />
        bool IShortcutPersistenceProvider.TryGetValue(string id, out IList<ShortcutBinding> bindings)
        {
            bindings = null;
            return Shortcuts?.TryGetValue(id, out bindings) ?? false;
        }

        /// <inheritdoc />
        void IShortcutPersistenceProvider.Persist(IDictionary<string, IList<ShortcutBinding>> blob)
        {
            Shortcuts = blob;
        }
    }
}