using System.Collections.Generic;

namespace HReader.Wpf.Shortcuts
{
    public interface IShortcutPersistenceProvider
    {
        bool TryGetValue(string id, out IList<ShortcutBinding> bindings);
        void Persist(IDictionary<string, IList<ShortcutBinding>> blob);
    }
}