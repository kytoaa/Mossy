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

        public TextEditorWindow(string address = "")
        {
            if (string.IsNullOrEmpty(address))
            {
                _viewModel = new TextEditorViewModel();
            }
            else
            {
                _viewModel = new TextEditorViewModel(address);
            }

            DataContext = _viewModel;
            //_viewModel.SetCursorPos += t => {
            //    TextInput.CaretIndex = t;
            //};

            InitializeComponent();
            CodeInput.TextCompositionEvent = _viewModel.OnTextChanged;
            _viewModel.Initialize();
        }
	}
}
