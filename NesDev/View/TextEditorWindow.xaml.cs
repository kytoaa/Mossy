using System;
using System.Diagnostics;
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
            TextInput.TextCompositionEvent = _viewModel.OnTextChanged;
        }
	}
}
