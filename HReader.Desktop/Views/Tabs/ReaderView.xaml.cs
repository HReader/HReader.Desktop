using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using HReader.Wpf.Shortcuts;
using HReader.Wpf.Shortcuts.Bindings;

namespace HReader.Views.Tabs
{
    /// <summary>
    /// Interaction logic for ReaderView.xaml
    /// </summary>
    public partial class ReaderView : UserControl
    {
        private readonly ShortcutManager shortcutManager;

        private ManualBindCollector binds;

        public ReaderView(ShortcutManager shortcutManager)
        {
            this.shortcutManager = shortcutManager;
            InitializeComponent();
            CreateShortcuts();
        }

        private void CreateShortcuts()
        {
            binds = new ManualBindCollector
            {
                shortcutManager.Bind("reader.fit-vertically",  s => new ManualBinder(s, FitVertically  )),
                shortcutManager.Bind("reader.fit-horizonally", s => new ManualBinder(s, FitHorizontally)),
                shortcutManager.Bind("reader.fit-original",    s => new ManualBinder(s, FitOriginal    )),
            };
        }
        
        private void FitHorizontally()
        {
            //TODO: figure out why this doesnt work
            BindingOperations.ClearBinding(Image, HeightProperty);
            Image.Stretch = Stretch.UniformToFill;
        }

        private void FitVertically()
        {
            var binding = new Binding
            {
                ElementName = "ScrollViewer",
                Path = new PropertyPath("ViewportHeight")
            };
            Image.SetBinding(HeightProperty, binding);
            Image.Stretch = Stretch.Uniform;
        }

        private void FitOriginal()
        {
            BindingOperations.ClearBinding(Image, HeightProperty);
            Image.Stretch = Stretch.None;
        }

        private void FitOriginal_Click(object sender, RoutedEventArgs e)
        {
            FitOriginal();
        }

        private void FitVertically_Click(object sender, RoutedEventArgs e)
        {
            FitVertically();
        }

        private void FitHorizontally_Click(object sender, RoutedEventArgs e)
        {
            FitHorizontally();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TODO: figure out how we're going to navigate on mouse input
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            binds.Process(e);
        }
    }
}
