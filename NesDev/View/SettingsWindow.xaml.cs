using Liken.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Liken.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel viewModel;

        public SettingsWindow()
        {
            viewModel = new SettingsViewModel();
            DataContext = viewModel;

            InitializeComponent();

            this.Closed += (_, _) => viewModel?.Close();
        }
    }
}
