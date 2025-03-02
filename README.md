# ChatView Control for MAUI.NET

The `ChatView` control is a highly customizable chat interface for MAUI.NET applications. It supports various features such as displaying messages, handling user interactions, managing replies, emoji reactions, avatars, and system messages. The control is optimized for native performance using platform-specific components like `RecyclerView` on Android and `UICollectionView` on iOS.

## Screenshots

| Android | iOS                |
|--------------|----------------------------|
| ![markdownview_screenshots](https://github.com/user-attachments/assets/a94b773b-9662-4e70-8913-e26c7d1dc1a6) | ![chatview_ios](https://github.com/user-attachments/assets/be809ff0-8429-436d-8571-20454e61e24d) |

![Indiko.Maui.Controls.Chat](nuget.png)

## Build Status
![ci](https://github.com/0xc3u/Indiko.Maui.Controls.Chat/actions/workflows/symanticrelease.yml/badge.svg)

## Installation

You can install the `Indiko.Maui.Controls.Chat` package via NuGet Package Manager or CLI:

[![NuGet](https://img.shields.io/nuget/v/Indiko.Maui.Controls.Chat.svg?label=NuGet)](https://www.nuget.org/packages/Indiko.Maui.Controls.Chat/)

### NuGet Package Manager
```bash
Install-Package Indiko.Maui.Controls.Chat
```

### .NET CLI
```bash
dotnet add package Indiko.Maui.Controls.Chat
```

---

### Adding to Your Project
To use the ChatView in your project, add the control to your app builder:

```csharp
using Microsoft.Maui.Hosting;
using Indiko.Maui.Controls.Chat;

var builder = MauiApp.CreateBuilder();
builder.UseChatView();
```

---

## Features

- **Message Display**: Renders text, image, video, and system messages.
- **Reply Support**: Reply-to-message functionality with previews of the original message.
- **Emoji Reactions**: Allows emoji reactions with reaction counts and participant details.
- **Avatars**: Displays sender avatars with customizable appearance.
- **Dynamic Separator**: Shows a customizable "New Messages" separator.
- **Customizable Styling**: Flexible styling for message backgrounds, text colors, fonts, and more.
- **Commands and Events**: Handles user interactions like taps, emoji reactions, and scrolls.
- **Scrollable Chat**: Supports smooth scrolling, including scroll-to-last-message and scroll-to-first-new-message.
- **Load More Messages**: Supports dynamic loading of older messages via a bound command.
- **Native Performance**: Uses `RecyclerView` on Android and `UICollectionView` on iOS for smooth performance.

---

## Supported Message Types

| Message Type | Description                |
|--------------|----------------------------|
| Text         | Standard text messages.    |
| Image        | Image-based messages.      |
| Video        | Video messages.            |
| System       | System-generated messages. |

---

## Models

### `ChatMessage`
Represents an individual message in the chat.

```csharp
public class ChatMessage
{
    public string MessageId { get; set; }
    public DateTime Timestamp { get; set; }
    public string TextContent { get; set; }
    public byte[] BinaryContent { get; set; }
    public bool IsOwnMessage { get; set; }
    public string SenderId { get; set; }
    public byte[] SenderAvatar { get; set; }
    public string SenderInitials { get; set; }
    public MessageType MessageType { get; set; }
    public MessageReadState ReadState { get; set; }
    public MessageDeliveryState DeliveryState { get; set; }
    public bool IsRepliedMessage => ReplyToMessage != null;
    public RepliedMessage ReplyToMessage { get; set; }
    public List<ChatMessageReaction> Reactions { get; set; } = [];
    public bool IsDateSeperator { get; set; }
}
```

### `ChatMessageReaction`
Represents reactions (emojis) on a message.

```csharp
public class ChatMessageReaction
{
    public string Emoji { get; set; }
    public int Count { get; set; }
    public List<string> ParticipantIds { get; set; } = new List<string>();
}
```

### `RepliedMessage`
Represents a replied message with a preview.

```csharp
public class RepliedMessage
{
    public string MessageId { get; set; }
    public string TextPreview { get; set; }
    public string SenderId { get; set; }

    public static string GenerateTextPreview(string text, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length > maxLength ? text[..maxLength] + "..." : text;
    }
}
```

### Enums

#### `MessageDeliveryState`
- `Sent`
- `Delivered`
- `Read`

#### `MessageReadState`
- `New`
- `Unread`
- `Read`

#### `MessageType`
- `Text`
- `Image`
- `Video`
- `System`

---

## Commands

| Command                          | Description                                                    |
|----------------------------------|----------------------------------------------------------------|
| `ScrolledCommand`                | Triggered when the chat view is scrolled. See example below.   |
| `ScrolledCommand`                | Triggered when the chat view is scrolled.                     |
| `MessageTappedCommand`           | Triggered when a message is tapped.                           |
| `AvatarTappedCommand`            | Triggered when an avatar is tapped.                           |
| `EmojiReactionTappedCommand`     | Triggered when an emoji reaction is tapped.                   |
| `LoadMoreMessagesCommand`        | Invoked when more messages need to be loaded.                 |
| `ScrolledToLastMessageCommand`   | Triggered when scrolled to the last message.                  |

---

## Styling

| Property                         | Default Value       | Description                                       |
|----------------------------------|---------------------|---------------------------------------------------|
| `SystemMessageBackgroundColor`   | LightYellow         | Background color for system messages.            |
| `SystemMessageTextColor`         | Red                | Text color for system messages.                 |
| `SystemMessageFontSize`          | 14                 | Font size for system messages.                  |
| `DateTextFontSize`               | 14                 | Font size for date separator text.              |
| `DateTextColor`                  | LightGray           | Color for date separator text.                   |
| `AvatarBackgroundColor`          | LightBlue           | Background color for avatars.                   |
| `AvatarTextColor`                | White              | Text color for avatar initials.                 |
| `OwnMessageBackgroundColor`      | LightBlue           | Background color for the user's messages.         |
| `OwnMessageTextColor`            | Black              | Text color for the user's messages.              |
| `OwnMessageFontSize`             | 14                 | Font size for the user's messages.               |
| `OtherMessageBackgroundColor`    | LightGray           | Background color for other users' messages.       |
| `OtherMessageTextColor`          | Black              | Text color for other users' messages.            |
| `OtherMessageFontSize`           | 14                 | Font size for other users' messages.             |
| `MessageFontSize`                | 14                 | Font size for messages.                          |
| `DateTextColor`                  | LightGray           | Color for date separator text.                   |
| `AvatarSize`                     | 36                 | Size of avatars.                                 |
| `ScrollToLastMessage`            | true               | Auto-scrolls to the last message.                |
| `ShowNewMessagesSeperator`       | false              | Enables or disables the new message separator.   |
| `EmojiReactionFontSize`          | 10                 | Font size for emoji reactions.                   |
| `ReplyMessageBackgroundColor`    | LightYellow         | Background color for replied message previews.   |
| `ReplyMessageFontSize`           | 10                 | Font size for replied message previews.          |
| `ReplyMessageTextColor`          | Black              | Text color for replied message previews.         |

---

## Usage

> **Platform-Specific Note:** The platform-specific code for iOS and Android uses a caching mechanism for images and video-based messages. The binary content of such messages is stored in the device's cache folder for optimized performance and memory management.

> **Note:** The `ChatView` control is solely responsible for rendering different message types. It does not include features like a text input box or a send button. These components need to be implemented by the user in the MAUI.NET app, as demonstrated in the `Indiko.Maui.Controls.Chat.Sample` project.

### Emoji Reaction Tapped Event Handling Example

To handle the `EmojiReactionTappedCommand` properly, you can define a method in your ViewModel as follows:

```csharp
[RelayCommand]
private void OnEmojiReactionTapped(ChatMessage message)
{
    Console.WriteLine($"Emoji Reaction tapped: {message.MessageId}");
}
```

### Message Clicked Event Handling Example

To handle the `MessageTappedCommand` properly, you can define a method in your ViewModel as follows:

```csharp
[RelayCommand]
private void OnMessageTapped(ChatMessage message)
{
    Console.WriteLine($"Message tapped for message: {message.MessageId}");
}
```

### Avatar Clicked Event Handling Example

To handle the `AvatarTappedCommand` properly, you can define a method in your ViewModel as follows:

```csharp
[RelayCommand]
private void OnAvatarTapped(ChatMessage message)
{
    Console.WriteLine($"Avatar tapped for message: {message.MessageId}");
}
```

### Scrolled Event Handling Example

To handle the `ScrolledCommand` properly, you can define a method in your ViewModel as follows:

```csharp
[RelayCommand]
private void Scrolled(ScrolledArgs scrolledArgs)
{
    // Handle scroll event logic
    Console.WriteLine($"Scrolled to position: X={scrolledArgs.X}, Y={scrolledArgs.Y}");
}
```

### XAML Example

```xml
xmlns:idk="clr-namespace:Indiko.Maui.Controls.Chat;assembly=Indiko.Maui.Controls.Chat"
...

<idk:ChatView Grid.Row="0" x:Name="chatView"
    
    OwnMessageBackgroundColor="{StaticResource Primary}" 
    OwnMessageTextColor="{StaticResource White}"
    OtherMessageBackgroundColor="{StaticResource Secondary}"
    OtherMessageTextColor="{StaticResource Black}"
    DateTextColor="{StaticResource Gray500}"
    DateTextFontSize="14"
    MessageTimeTextColor="{StaticResource Gray200}"
    NewMessagesSeperatorTextColor="{StaticResource Primary}"
    NewMessagesSeperatorFontSize="16"
    NewMessagesSeperatorText="New Messages"
    AvatarTextColor="{StaticResource White}"
    AvatarBackgroundColor="{StaticResource Tertiary}"
    Messages="{Binding ChatMessages}"
    EmojiReactionFontSize="14"
    EmojiReactionTextColor="{StaticResource Primary}"
    ReplyMessageBackgroundColor="{StaticResource Tertiary}"
    ReplyMessageTextColor="{StaticResource White}"
    LoadMoreMessagesCommand="{Binding LoadOlderMessagesCommand}"
    ScrolledCommand="{Binding ScrolledCommand}"
    AvatarTappedCommand="{Binding AvatarTappedCommand}"
    MessageTappedCommand="{Binding MessageTappedCommand}"
    EmojiReactionTappedCommand="{Binding EmojiReactionTappedCommand}"
    SendIcon="send.png"
    DeliveredIcon="check.png"
    ReadIcon="read.png"
    ScrollToFirstNewMessage="True"
    ShowNewMessagesSeperator="True"
    ScrolledToLastMessageCommand="{Binding ScrolledToLastMessageCommand}"
    SystemMessageBackgroundColor="{StaticResource Yellow300Accent}"
    SystemMessageTextColor="{StaticResource Tertiary}"
    SystemMessageFontSize="14">

</idk:ChatView>
```

### Code-Behind Example

```csharp
var chatView = new ChatView
{
    Messages = new ObservableRangeCollection<ChatMessage>(),
    MessageTappedCommand = new Command<ChatMessage>(OnMessageTapped),
    AvatarTappedCommand = new Command(OnAvatarTapped),
    LoadMoreMessagesCommand = new Command(OnLoadMoreMessages),
    OwnMessageBackgroundColor = Colors.LightBlue,
    OtherMessageBackgroundColor = Colors.LightGray,
    ShowNewMessagesSeperator = true,
    NewMessagesSeperatorText = "New Messages"
};

void OnMessageTapped(ChatMessage message)
{
    // Handle message tap
}

void OnAvatarTapped()
{
    // Handle avatar tap
}

void OnLoadMoreMessages()
{
    // Load older messages
}
```

---

## Contributing

We encourage you to contribute to the development of the `ChatView` control! Whether you're fixing bugs, adding new features, or enhancing the documentation, your contributions make a difference.

If you find the `ChatView` control helpful, please consider leaving a ⭐ on the repository. It helps others discover this project and shows your support!

Contributions are welcome! Please follow the guidelines for creating feature branches, writing commit messages, and submitting pull requests.

---

# How to Contribute

Thank you for considering contributing to our project! Please follow these guidelines to ensure a smooth process.

## 1. Work on a Feature Branch

Always create a new branch for your feature or fix. This keeps the main branch clean and makes it easier to manage changes.

```bash
git checkout -b feature/your-feature-name
```

## 2. Start a Pull Request

Once your feature is complete, push your branch to the repository and start a pull request to merge it into the main branch. Ensure all tests pass and your code follows the project's coding standards.

```bash
git push origin feature/your-feature-name
```

Then, create a pull request on GitHub and provide a clear description of your changes.

## 3. Use Semantic Release Prefixes for Commits

When committing your changes, use semantic release prefixes to categorize your commits. This helps in generating automated release notes and versioning.

The commit contains the following structural elements to communicate intent to the consumers of your library:

- **fix:** a commit of the type fix patches a bug in your codebase (this correlates with PATCH in Semantic Versioning).
- **feat:** a commit of the type feat introduces a new feature to the codebase (this correlates with MINOR in Semantic Versioning).
- **BREAKING CHANGE:** a commit that has a footer BREAKING CHANGE:, or appends a ! after the type/scope, introduces a breaking API change (correlating with MAJOR in Semantic Versioning). A BREAKING CHANGE can be part of commits of any type.
- Types other than fix: and feat: are allowed. For example, @commitlint/config-conventional (based on the Angular convention) recommends:
  - **build:** Changes that affect the build system or external dependencies
  - **chore:** Other changes that don't modify src or test files
  - **ci:** Changes to our CI configuration files and scripts
  - **docs:** Documentation only changes
  - **style:** Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
  - **refactor:** A code change that neither fixes a bug nor adds a feature
  - **perf:** A code change that improves performance
  - **test:** Adding missing tests or correcting existing tests

Footers other than BREAKING CHANGE: <description> may be provided and follow a convention similar to git trailer format. Additional types are not mandated by the Conventional Commits specification and have no implicit effect in Semantic Versioning (unless they include a BREAKING CHANGE). A scope may be provided to a commit’s type, to provide additional contextual information and is contained within parenthesis, e.g., feat(parser): add ability to parse arrays.

Example commit messages:

```bash
git commit -m "fix: resolve issue with user authentication"
git commit -m "feat: add new payment gateway integration"
git commit -m "BREAKING CHANGE: update API endpoints"
```

## 4. Write Meaningful Commit Messages

Commit messages should be concise yet descriptive. They should explain the "what" and "why" of your changes.

- **Good Example:** `fix: correct typo in user profile page`
- **Bad Example:** `fixed stuff`

## Additional Tips

- Ensure your code adheres to the project's coding standards and guidelines.
- Include tests for new features or bug fixes.
- Keep your commits atomic; a single commit should represent a single logical change.
- Update the documentation to reflect any new features or changes.

We appreciate your contributions and look forward to your pull requests!

Happy coding!

---

## License

This project is licensed under the MIT License.

