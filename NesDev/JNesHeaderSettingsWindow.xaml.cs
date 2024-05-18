using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NesDev
{
    /// <summary>
    /// Interaction window for setting iNES header settings
    /// Named JNes rather than INes to avoid confusion with interfaces
    /// </summary>
    public partial class JNesHeaderSettingsWindow : Window
    {
        public JNesHeaderSettingsWindow()
        {
            InitializeComponent();
        }
        public void OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("hey");
        }
    }
}
