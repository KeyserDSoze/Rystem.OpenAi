using System.Collections.Generic;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for building message threads with various customization options.
    /// </summary>
    public interface IMessageThreadBuilder<T>
    {
        /// <summary>
        /// Gets the builder instance.
        /// </summary>
        T Thread { get; }
        /// <summary>
        /// Adds an empty message to the thread for a specific chat role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        IMessageThreadBuilder<T> AddEmpty(ChatRole role);

        /// <summary>
        /// Adds a text message to the thread for a specific chat role.
        /// </summary>
        /// <param name="role">The chat role (e.g., user, assistant).</param>
        /// <param name="text">The text content to add.</param>
        /// <returns>The builder instance.</returns>
        T AddText(ChatRole role, string text);

        /// <summary>
        /// Adds a content section to the thread for a specific chat role.
        /// </summary>
        /// <param name="role">The chat role (default is user).</param>
        /// <returns>A builder for customizing the content.</returns>
        ChatMessageContentBuilder<IMessageThreadBuilder<T>> AddContent(ChatRole role = ChatRole.User);

        /// <summary>
        /// Adds a user-specific content section to the thread.
        /// </summary>
        /// <returns>A builder for customizing the user content.</returns>
        ChatMessageContentBuilder<IMessageThreadBuilder<T>> AddUserContent();

        /// <summary>
        /// Adds an assistant-specific content section to the thread.
        /// </summary>
        /// <returns>A builder for customizing the assistant content.</returns>
        ChatMessageContentBuilder<IMessageThreadBuilder<T>> AddAssistantContent();

        /// <summary>
        /// Adds an attachment to the thread for a specific chat role.
        /// </summary>
        /// <param name="fileId">The file identifier for the attachment.</param>
        /// <param name="withCodeInterpreter">Indicates if the code interpreter tool is included.</param>
        /// <param name="withFileSearch">Indicates if the file search tool is included.</param>
        /// <returns>The builder instance.</returns>
        IMessageThreadBuilder<T> AddAttachment(string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false);

        /// <summary>
        /// Adds metadata to the last message.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The builder instance.</returns>
        IMessageThreadBuilder<T> AddMetadata(string key, string value);

        /// <summary>
        /// Adds a dictionary of metadata to the last message.
        /// </summary>
        /// <param name="metadata">The metadata dictionary to add.</param>
        /// <returns>The builder instance.</returns>
        IMessageThreadBuilder<T> AddMetadata(Dictionary<string, string> metadata);

        /// <summary>
        /// Clears all metadata from the last message.
        /// </summary>
        /// <returns>The builder instance.</returns>
        IMessageThreadBuilder<T> ClearMetadata();

        /// <summary>
        /// Removes specific metadata by key from the last message.
        /// </summary>
        /// <param name="key">The metadata key to remove.</param>
        /// <returns>The builder instance.</returns>
        IMessageThreadBuilder<T> RemoveMetadata(string key);
    }
}
