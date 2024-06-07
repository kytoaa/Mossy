using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Liken.View;

public class CodeBox : TextBox
{
	public Action<ViewModel.TextEvent, Action, Action<int>>? TextCompositionEvent;
	private void InvokeComposition(ViewModel.TextEvent textEvent, RoutedEventArgs e) => TextCompositionEvent?.Invoke(textEvent, () => e.Handled = true, c => CaretIndex = c);

	private readonly Key[] Keys = [Key.Space, Key.Enter, Key.Back, Key.Tab, Key.Left, Key.Down, Key.Right, Key.Up];

	protected override void OnInitialized(EventArgs e)
	{
		base.OnInitialized(e);
	}

	protected override void OnPreviewTextInput(TextCompositionEventArgs e)
	{
		InvokeComposition(new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Text, e.Text), e);
		base.OnPreviewTextInput(e);
	}
	protected override void OnPreviewKeyDown(KeyEventArgs e)
	{
		bool isKey = Keys.Contains(e.Key);

		if (isKey)
		{
			ViewModel.TextEvent textEvent;
			switch (e.Key)
			{
				case Key.Space:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Space);
					break;
				case Key.Enter:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Enter);
					break;
				case Key.Back:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Backspace);
					break;
				case Key.Tab:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Tab);
					break;
				case Key.Left:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Left);
					break;
				case Key.Right:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Right);
					break;
				case Key.Up:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Up);
					break;
				case Key.Down:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Down);
					break;
				default:
					textEvent = new ViewModel.TextEvent(ViewModel.TextEvent.EventType.Error);
					break;
			}
			InvokeComposition(textEvent, e);
		}

		base.OnPreviewKeyDown(e);
	}
	protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		base.OnPreviewMouseLeftButtonDown(e);
		if (IsFocused)
		{
			int index = GetCharacterIndexFromPoint(e.GetPosition(this), true);
			InvokeComposition(new ViewModel.TextEvent(ViewModel.TextEvent.EventType.CursorPos, index.ToString()), e);
		}
	}
}