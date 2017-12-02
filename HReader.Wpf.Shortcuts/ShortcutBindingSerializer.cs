using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HReader.Wpf.Shortcuts
{
    public  static class ShortcutBindingSerializer
    {
        private static readonly char[] Plus = {'+'};

        private static ModifierKeys DeserializeModifier(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "alt":
                    return ModifierKeys.Alt;
                case "ctrl":
                case "control":
                case "strg":
                    return ModifierKeys.Control;
                case "shift":
                    return ModifierKeys.Shift;

                case "win":
                case "windows":
                case "super":
                case "command":
                case "start":
                    return ModifierKeys.Windows;
                default:
                    return ModifierKeys.None;
            }
        }

        private static IEnumerable<string> SerializeModifiers(ModifierKeys value)
        {
            return value == ModifierKeys.None 
                ? Enumerable.Empty<string>()
                : DoSerialize();

            IEnumerable<string> DoSerialize()
            {
                if ((value & ModifierKeys.Control) > 0) yield return "Ctrl";
                if ((value & ModifierKeys.Alt    ) > 0) yield return "Alt";
                if ((value & ModifierKeys.Shift  ) > 0) yield return "Shift";
                if ((value & ModifierKeys.Windows) > 0) yield return "Win";
            }
        }

        private static Key DeserializeKey(string value)
        {
            if (Enum.TryParse<Key>(value, true, out var k)) return k;
            throw new InvalidOperationException($"Not a valid key: {value}");
        }

        public static string Serialize(ShortcutBinding value)
        {
            var modifiers = string.Join("+", SerializeModifiers(value.Modifiers));

            return modifiers == string.Empty 
                ? value.Key.ToString() 
                : $"{modifiers}+{value.Key}";
        }

        public static ShortcutBinding Deserialize(string value)
        {
            var values = value.Split(Plus, StringSplitOptions.RemoveEmptyEntries);

            switch (values.Length)
            {
                case 0: return new ShortcutBinding(Key.None);
                case 1: return new ShortcutBinding(DeserializeKey(values[0]));
            }

            var key = DeserializeKey(values.Last());

            var modifiers = values.Take(values.Length - 1)
                                  .Select(s => DeserializeModifier(s.Trim()))
                                  .Aggregate(ModifierKeys.None, (curr, val) => curr | val);

            return new ShortcutBinding(key, modifiers);
        }
    }
}
