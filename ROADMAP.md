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

## P0 — Parity / correctness

- ☑ **Voice notes + Android audio parity** [Render][Model] — Android had no audio
  branch at all; iOS audio was a bare "Play" button. Now a real voice-note cell on both
  platforms: circular play/pause, tap-to-seek waveform (real samples via
  `ChatMessage.AudioWaveform`, else a stable pseudo-waveform), elapsed/total duration.
  Added `ChatMessage.AudioDuration` and `ChatMessage.AudioWaveform`.
  iOS verified end-to-end (render + play/pause + progress + duration + seek). Android
  rendering verified; also fixed a pre-existing RecyclerView crash where LoadMore firing
  during layout mutated the adapter (now deferred to the next frame).
- ☐ **Media captions** [Render][Model] — image/video bubbles can't show text. Add a
  caption under the media in the same bubble (`ChatMessage.Caption` or reuse `TextContent`).

## P1 — Table stakes for a common chat control

- ☐ **Sender name in group chats** [Render][Model] — name label above other-people's
  bubbles; consecutive-message grouping. Add `ChatMessage.SenderName`.
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

- ☐ Full-screen image viewer w/ pinch-zoom; media download progress; blurhash placeholder [Render]
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
