using System.CodeDom;

namespace NesDev.ViewModel;

public class TextEvent
{
	public readonly string? Text;
	public readonly EventType Type;

	public TextEvent(EventType type, string? text = null)
	{
		if (type == EventType.Text && text == null)
			throw new ArgumentException(nameof(text) + " cannot be null when type is text");
		if ((type != EventType.Text && type != EventType.CursorPos) && text != null)
			throw new ArgumentException(nameof(text) + " must be null when type is not text");
		if (type == EventType.Error)
			throw new ArgumentException(nameof(type) + " is of type Error");

		Type = type;
		Text = text;
	}

	public enum EventType
	{
		Text,
		Space,
		Backspace,
		Enter,
		Tab,
		Left,
		Right,
		Up,
		Down,
		CursorPos,
		Error,
	}
}