namespace NesDevCompiler.CharacterStream;

/// <summary>
/// Represents an abstract Character Stream.
/// </summary>
public interface ICharacterStream
{
	/// <summary>
	/// View the next character in the stream without removing it.
	/// </summary>
	/// <returns> The next character in the stream. </returns>
	char Peek();
	/// <summary>
	/// View the character c ahead in the stream.
	/// </summary>
	/// <param name="c"> The index of the character to view. </param>
	/// <returns> The character c ahead. </returns>
	char Peek(int c);
	/// <summary>
	/// Views and removes the next character in the stream.
	/// </summary>
	/// <returns> The character removed from the stream. </returns>
	char Read();
	/// <summary>
	/// Views and removes the character c ahead in the stream.
	/// </summary>
	/// <param name="c"> The index of the character to view and remove. </param>
	/// <returns> The character removed from the stream. </returns>
	char Read(int c);
	/// <summary>
	/// Checks if the reader has reached the end of the stream.
	/// </summary>
	/// <returns> Whether the reader has reached the end of the stream. </returns>
	bool End();
}