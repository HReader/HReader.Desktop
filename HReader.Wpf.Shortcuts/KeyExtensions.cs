using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HReader.Wpf.Shortcuts
{
    public static class KeyExtensions
    {
        internal static bool In(this Key @this, params Key[] set)
        {
            return set.Contains(@this);
        }


        public static ModifierKeys Modifiers(this KeyEventArgs @this)
        {
            var modifiers = @this.KeyboardDevice.Modifiers;

            if (@this.KeyboardDevice.IsKeyDown(Key.LWin) || @this.KeyboardDevice.IsKeyDown(Key.RWin))
            {
                modifiers |= ModifierKeys.Windows;
            }

            return modifiers;
        }
    }
}
