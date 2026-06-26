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

Interaction & correctness
- ☑ **Long-press to react on every message type** [Render] — text, image, video, voice note
  (fixed the `_message`-never-assigned bug on the iOS audio/video cells).
- ☑ **iOS inverted-list scroll fixes** [Render] — correct initial position (newest at bottom,
  no swoosh), stable load-more (no jump), no startup crash (diffable identity via `IsEqual`/
  `GetNativeHash`).
- ☑ **`ObservableRangeCollection.InsertRange`** [Render] — prepend older messages for
  infinite-scroll load-more.
- ☑ **Android RecyclerView crash fix** [Render] — adapter mutations during LoadMore/scroll
  are marshalled off the layout pass.

New bindable properties: `OpenVideoFullScreen`, `OpenImageFullScreen`.

---

## P1 — Table stakes for a common chat control

- ☐ **Clickable links + data detectors** [Render] — URLs/phones/emails tappable in text.
- ☐ **Link previews (URL unfurling)** [Render][Model] — title/description/thumbnail card.
- ☐ **Swipe-to-reply gesture** [Render] — swipe a bubble to trigger reply.
- ☐ **Tap reply-preview → jump to original** [Render].
- ☐ **Documents / files** [Render][Model] — filename + size + icon (`FileName`, `MimeType`, `FileSize`).
- ☐ **Edited / forwarded indicators** [Render][Model] — `IsEdited`, `IsForwarded`.
- ☐ **Scroll-to-bottom FAB with unread count** [Render].
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

1. **Clickable links + data detectors** (P1) — cheap, high daily-use payoff.
2. **Swipe-to-reply gesture** (P1) — standard reply interaction.
3. **Link previews / documents** (P1) — rounds out content-type coverage.
