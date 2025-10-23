# Class: ChatMessageViewHolder

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Platforms.Android` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Platforms/Android/ChatMessageViewHolder.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public class ChatMessageViewHolder : RecyclerView.ViewHolder, IDisposable
```

## Relationships

**Inherits from:** [RecyclerView.ViewHolder](RecyclerView.ViewHolder.md)

**Implements:**
- [IDisposable](IDisposable.md)

**Dependencies:**
- [Android.Views.View](Android.Views.View.md)
- [Android.Views.View.LongClickEventArgs](Android.Views.View.LongClickEventArgs.md)
- [ChatMessage](ChatMessage.md)
- [ChatView](ChatView.md)
- [ChatViewHandler](ChatViewHandler.md)
- [EventHandler](EventHandler.md)
- [EventHandler<>](EventHandler__.md)
- [FrameLayout](FrameLayout.md)
- [IDisposable](IDisposable.md)
- [ImageView](ImageView.md)
- [LinearLayout](LinearLayout.md)
- [RecyclerView.ViewHolder](RecyclerView.ViewHolder.md)
- [TextView](TextView.md)
- [VideoView](VideoView.md)
- [void](void.md)

## Constructors

### ChatMessageViewHolder

```csharp
public .ctor(Android.Views.View itemView, TextView dateTextView, TextView textView, ImageView imageView, FrameLayout videoContainer, VideoView videoView, TextView timestampTextView, FrameLayout frameLayout, TextView newMessagesSeparatorTextView, ImageView avatarView, LinearLayout reactionContainer, ImageView deliveryStatusIcon, LinearLayout replySummaryFrame, TextView replyPreviewTextView, TextView replySenderTextView, TextView systemMessageTextView)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `itemView` | `Android.Views.View` |  | `` |
| `dateTextView` | `TextView` |  | `` |
| `textView` | `TextView` |  | `` |
| `imageView` | `ImageView` |  | `` |
| `videoContainer` | `FrameLayout` |  | `` |
| `videoView` | `VideoView` |  | `` |
| `timestampTextView` | `TextView` |  | `` |
| `frameLayout` | `FrameLayout` |  | `` |
| `newMessagesSeparatorTextView` | `TextView` |  | `` |
| `avatarView` | `ImageView` |  | `` |
| `reactionContainer` | `LinearLayout` |  | `` |
| `deliveryStatusIcon` | `ImageView` |  | `` |
| `replySummaryFrame` | `LinearLayout` |  | `` |
| `replyPreviewTextView` | `TextView` |  | `` |
| `replySenderTextView` | `TextView` |  | `` |
| `systemMessageTextView` | `TextView` |  | `` |

## Fields

### _avatarClickHandler

```csharp
private EventHandler _avatarClickHandler
```

**Returns:** `EventHandler`

### _textBubbleClickHandler

```csharp
private EventHandler _textBubbleClickHandler
```

**Returns:** `EventHandler`

### _imageBubbleClickHandler

```csharp
private EventHandler _imageBubbleClickHandler
```

**Returns:** `EventHandler`

### _videoBubbleClickHandler

```csharp
private EventHandler _videoBubbleClickHandler
```

**Returns:** `EventHandler`

### _emojiReactionClickHandler

```csharp
private EventHandler _emojiReactionClickHandler
```

**Returns:** `EventHandler`

### _longPressHandler

```csharp
private EventHandler<Android.Views.View.LongClickEventArgs> _longPressHandler
```

**Returns:** `EventHandler<Android.Views.View.LongClickEventArgs>`

## Properties

### DateTextView

```csharp
public TextView DateTextView { get; }
```

**Returns:** `TextView`

### TextView

```csharp
public TextView TextView { get; }
```

**Returns:** `TextView`

### ImageView

```csharp
public ImageView ImageView { get; }
```

**Returns:** `ImageView`

### VideoView

```csharp
public VideoView VideoView { get; }
```

**Returns:** `VideoView`

### VideoContainer

```csharp
public FrameLayout VideoContainer { get; }
```

**Returns:** `FrameLayout`

### TimestampTextView

```csharp
public TextView TimestampTextView { get; }
```

**Returns:** `TextView`

### FrameLayout

```csharp
public FrameLayout FrameLayout { get; }
```

**Returns:** `FrameLayout`

### NewMessagesSeparatorTextView

```csharp
public TextView NewMessagesSeparatorTextView { get; }
```

**Returns:** `TextView`

### AvatarView

```csharp
public ImageView AvatarView { get; }
```

**Returns:** `ImageView`

### ReactionContainer

```csharp
public LinearLayout ReactionContainer { get; }
```

**Returns:** `LinearLayout`

### DeliveryStatusIcon

```csharp
public ImageView DeliveryStatusIcon { get; }
```

**Returns:** `ImageView`

### ReplySummaryFrame

```csharp
public LinearLayout ReplySummaryFrame { get; }
```

**Returns:** `LinearLayout`

### ReplySenderTextView

```csharp
public TextView ReplySenderTextView { get; }
```

**Returns:** `TextView`

### ReplyPreviewTextView

```csharp
public TextView ReplyPreviewTextView { get; }
```

**Returns:** `TextView`

### SystemMessageTextView

```csharp
public TextView SystemMessageTextView { get; }
```

**Returns:** `TextView`

## Methods

### AttachEventHandlers

```csharp
public void AttachEventHandlers(ChatMessage message, ChatView chatView, ChatViewHandler handler)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `message` | `ChatMessage` |  | `` |
| `chatView` | `ChatView` |  | `` |
| `handler` | `ChatViewHandler` |  | `` |

### DetachEventHandlers

```csharp
public void DetachEventHandlers()
```

### ApplyVisualFeedbackToChatBubble

```csharp
public async void ApplyVisualFeedbackToChatBubble()
```

### ApplyVisualFeedbackToEmojiReaction

```csharp
public async void ApplyVisualFeedbackToEmojiReaction()
```

### ApplyVisualFeedbackToAvatar

```csharp
public async void ApplyVisualFeedbackToAvatar()
```

### Dispose

```csharp
public void Dispose()
```

