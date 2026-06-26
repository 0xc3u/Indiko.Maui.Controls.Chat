# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build the library
dotnet build src/Indiko.Maui.Controls.Chat.sln -c Release

# Build the sample app
dotnet build samples/Indiko.Maui.Controls.Chat.Sample.sln -c Release

# Regenerate API documentation
dotnet run --project tools/Tools.CodeDocGenerator/Tools.CodeDocGenerator.csproj
```

There are no automated tests in this repository. NuGet publishing is handled automatically by CI when a version tag is pushed.

## Architecture

This is a .NET MAUI control library that ships as a NuGet package (`Indiko.Maui.Controls.Chat`). It targets `net10.0-android` and `net10.0-ios` only (min OS: Android 30, iOS 14.2). The package version is set in the library `.csproj` and bumped automatically by Semantic Release CI on tagged commits — do not edit it by hand.

### Layer Structure

```
ChatView (cross-platform MAUI View)
    └── Platform Handlers (map ChatView → native UI)
            ├── Android: RecyclerView + ChatMessageAdapter (ViewHolder pattern)
            └── iOS: UICollectionView + ChatViewDataSource/ChatViewDelegate
```

**`ChatView.cs`** — The single cross-platform control. Defines 40+ `BindableProperty` fields for styling, a `Messages` property of type `ObservableRangeCollection<ChatMessage>`, and command properties (`ScrolledCommand`, `MessageTappedCommand`, `AvatarTappedCommand`, `EmojiReactionTappedCommand`, `LoadMoreMessagesCommand`, `ScrolledToLastMessageCommand`, `LongPressedCommand`).

**`BuilderExtension.cs`** — Registers platform handlers via `UseChatView()` called in consumer apps' `MauiProgram.cs`.

**`Models/`** — `ChatMessage`, `ChatMessageReaction`, `RepliedMessage`, `ContextAction`, `ContextMenuItem`, `ObservableRangeCollection<T>`, and enums (`MessageType`, `MessageDeliveryState`, `MessageReadState`). `MessageType` is `Text`, `Image`, `Video`, `Audio`, `System`, `Date` — each non-system type has its own `Own*`/`Other*` cell on iOS and its own ViewHolder branch on Android. The README's "Supported Message Types" table is stale (omits `Audio`); trust the enum.

**`Platforms/Android/`** — Handler, RecyclerView adapter, ViewHolder, scroll listener, blur overlay, and bitmap/pixel utilities.

**`Platforms/iOS/`** — Handler, UICollectionView data source and delegate, custom flow layout, per-type message cells (`Own*MessageCell`, `Other*MessageCell`, `SystemMessageCell`, `DateGroupSeperatorCell`), context menu view, and UIKit extensions.

### Key Design Decisions

- All new `ChatView` properties must be declared as `BindableProperty` in `ChatView.cs` and mapped in both platform handlers via the `PropertyMapper`.
- `ObservableRangeCollection<T>` is used instead of `ObservableCollection<T>` to support bulk `AddRange`/`ReplaceRange` without per-item change notifications — essential for scroll performance.
- Platform handlers use `ConnectHandler`/`DisconnectHandler` lifecycle methods. On iOS, use a `Proxy` inner class with `WeakReference<T>` to the virtual view to avoid circular reference memory leaks (C# objects subclassing `NSObject` do not get GC'd when circular refs exist on Apple platforms).
- The iOS `UICollectionView` is **inverted**: the view itself carries a `CGAffineTransform.MakeScale(1, -1)` and each cell's `ContentView` carries the inverse transform to render upright. This puts the newest message (`Messages[0]`) at the visual bottom with `contentOffset.Y = 0`, so new messages appear without programmatic scrolling. Consequence: data-source indices are reversed relative to `Messages` — convert with `Messages.Count - 1 - index` when mapping between a model index and an `NSIndexPath` row.

## Code Style

- Allman braces, 4-space indentation, 120-character line limit.
- `using` statements outside the namespace; remove unused usings.
- Commit messages must use Semantic Release prefixes: `feat:`, `fix:`, `docs:`, `chore:`, `refactor:`, `perf:`, `style:`, `ci:`, `build:`, `test:`.

## MAUI-Specific Rules

- Prefer `Grid` over nested layouts to keep the visual tree flat.
- Use `VerticalStackLayout`/`HorizontalStackLayout`, not `StackLayout`.
- Use `Border`, not `Frame`.
- Never nest scrollable controls (`ScrollView`, `CollectionView`) within each other unless they scroll in different directions.
- Handler registration belongs in `MauiProgram.cs` via `builder.ConfigureMauiHandlers(...)`.
