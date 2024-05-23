using System;
using System.Windows;
using System.Windows.Navigation;
using NesDev.ViewModel;

namespace NesDev.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TextEditorWindow : Window
    {
        private TextEditorViewModel _viewModel;

        public TextEditorWindow()
        {
            _viewModel = new TextEditorViewModel();

            DataContext = _viewModel;
            //_viewModel.SetCursorPos += t => {
            //    TextInput.CaretIndex = t;
            //};

            InitializeComponent();

            _viewModel.Text = "Hello, World!";
        }

        private void TextChangedEventHandler(object sender, System.Windows.Controls.TextChangedEventArgs e) { }//_viewModel.TextChangedEventHandler(sender, e);
	}
}
