# ROADMAP

Feature roadmap for `Indiko.Maui.Controls.Chat`, derived from a gap analysis
against mainstream messengers (WhatsApp, Telegram, Signal, iMessage).

## Scope

This is a **rendering control** — it intentionally ships no composer, networking,
persistence, or encryption. Items are tagged:

- **[Render]** — belongs in this control
- **[Model]** — needs `ChatMessage` / model additions
- **[App]** — consumer responsibility, out of scope (E2E, calls, status/stories, sync, push)

Status: ☐ todo · ◐ in progress · ☑ done

---

## ✅ Shipped

Media
- ☑ **Voice notes + Android audio parity** [Render][Model] — real voice-note cell on both
  platforms (play/pause, tap-to-seek waveform, duration). Added `ChatMessage.AudioDuration`
  and `ChatMessage.AudioWaveform`. *(released in 1.3.0)*
- ☑ **Tap-to-play video** [Render] — blurred first-frame poster + play button; no auto-play
  on scroll.
- ☑ **Full-screen video** [Render] — opens full screen with native play/pause + seek; opt-out
  to inline via `OpenVideoFullScreen` (default true).
- ☑ **Full-screen image viewer** [Render] — pinch-to-zoom, pan, double-tap; opt-out via
  `OpenImageFullScreen` (default true).
- ☑ **Aspect-sized image bubbles** [Render] — images/videos no longer blow up the bubble;
  image-only messages (no caption required) can be sent.
- ☑ **Media captions** [Render] — image/video messages show their `TextContent` as a caption
  under the media in the same bubble.
- ☑ **Sender name + consecutive grouping** [Render][Model] — `ChatMessage.SenderName` shown
  above incoming bubbles (all message types), de-duplicated for consecutive messages from the
  same sender. New properties: `ShowSenderName`, `SenderNameTextColor`, `SenderNameFontSize`.
- ☑ **Clickable links + data detectors** [Render] — URLs, phone numbers and emails in text
  messages are detected and tappable (open browser/dialer/mail). Long-press-to-react still
  works. New properties: `DetectLinks`, `LinkTextColor`.

Interaction & correctness
- ☑ **Scroll-to-bottom button + unread badge** [Render] — a floating button appears when the user
  scrolls away from the newest message and jumps back on tap; an optional badge counts messages
  arriving while scrolled up. Fully styleable (background/icon/size/margin + badge colors/font) via
  bindable properties; toggle with `ShowScrollToBottomButton` / `ShowScrollToBottomBadge`. Also
  fixed a latent Android bug where `Messages.CollectionChanged` was subscribed once per mapped
  property.
- ☑ **Swipe-to-reply gesture** [Render] — swipe a bubble to the right to trigger a reply. Raises
  the same event as the context menu's "Reply" item (`LongPressedCommand` with a `ContextAction`
  named "reply") so consumers handle reply once; springs the row back (haptic on iOS, clamped
  slide on Android). New properties: `EnableSwipeToReply`, `SwipeReplyActionName`.
- ☑ **Long-press to react on every message type** [Render] — text, image, video, voice note
  (fixed the `_message`-never-assigned bug on the iOS audio/video cells).
- ☑ **iOS inverted-list scroll fixes** [Render] — correct initial position (newest at bottom,
  no swoosh), stable load-more (no jump), no startup crash (diffable identity via `IsEqual`/
  `GetNativeHash`).
- ☑ **`ObservableRangeCollection.InsertRange`** [Render] — prepend older messages for
  infinite-scroll load-more.
- ☑ **Android RecyclerView crash fix** [Render] — adapter mutations during LoadMore/scroll
  are marshalled off the layout pass.

New bindable properties: `OpenVideoFullScreen`, `OpenImageFullScreen`, `EnableSwipeToReply`,
`SwipeReplyActionName`, `ShowScrollToBottomButton`, `ScrollToBottomButtonBackgroundColor`,
`ScrollToBottomButtonIconColor`, `ScrollToBottomButtonSize`, `ScrollToBottomButtonMargin`,
`ShowScrollToBottomBadge`, `ScrollToBottomBadgeBackgroundColor`, `ScrollToBottomBadgeTextColor`,
`ScrollToBottomBadgeFontSize`.

---

## P1 — Table stakes for a common chat control

- ☐ **Link previews (URL unfurling)** [Render][Model] — title/description/thumbnail card.
- ☐ **Tap reply-preview → jump to original** [Render].
- ☐ **Documents / files** [Render][Model] — filename + size + icon (`FileName`, `MimeType`, `FileSize`).
- ☐ **Edited / forwarded indicators** [Render][Model] — `IsEdited`, `IsForwarded`.
- ☐ **Reaction details** [Render] — show who reacted (model already has `ParticipantIds`).
- ☐ **Typing indicator bubble** [Render].

## P2 — Polish / advanced

- ☐ Media download progress + blurhash/placeholder while loading [Render]
- ☐ Animated GIF & sticker support [Render][Model]
- ☐ Jumbo emoji (large rendering for emoji-only messages) [Render]
- ☐ Bubble tails/notches [Render]
- ☐ Multi-select (bulk delete/forward) [Render]
- ☐ In-conversation search + highlight; jump-to-date [Render]
- ☐ Location / contact card / poll message types [Render][Model]
- ☐ Pinned-messages bar; starred messages [Render][Model]
- ☐ RTL support + VoiceOver/TalkBack labels + Dynamic Type audit [Render]
- ☐ Chat wallpaper / background [Render]

## Strategic

- ☐ Optional `ChatInputView` composer (attachments, voice recording, emoji picker) [Render]
  — lowers adoption friction; most consumers reimplement this today.

---

## Recommended next

1. **Link previews / documents** (P1) — rounds out content-type coverage.
2. **Tap reply-preview → jump to original** (P1) — pairs naturally with swipe-to-reply.
3. **Typing indicator bubble** (P1).
