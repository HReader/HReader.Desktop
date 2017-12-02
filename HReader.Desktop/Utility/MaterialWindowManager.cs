using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Caliburn.Micro;

namespace HReader.Utility
{
    internal class MaterialWindowManager : WindowManager
    {
        /// <inheritdoc />
        protected override Window CreateWindow(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            var window = base.CreateWindow(rootModel, isDialog, context, settings);

            window.SetResourceReference(TextElement.ForegroundProperty, "MaterialDesignBody");
            window.SetResourceReference(Control.BackgroundProperty, "MaterialDesignPaper");
            TextElement.SetFontWeight(window, FontWeights.Medium);
            TextElement.SetFontSize(window, 14);
            TextElement.SetFontFamily(window, new FontFamily(new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Resources/Roboto/"), "Roboto"));

            return window;
        }
    }
}
