using Microsoft.Maui.Graphics;

namespace Indiko.Maui.Controls.Chat;

/// <summary>
/// A lightweight live waveform shown while recording a voice note. Amplitude samples (0..1) are
/// pushed in by <see cref="ChatInputView"/> from the platform recorder; bars scroll in from the right.
/// </summary>
internal sealed class RecordingWaveformView : GraphicsView
{
    private readonly WaveDrawable _drawable = new();

    public RecordingWaveformView()
    {
        Drawable = _drawable;
        HeightRequest = 28;
    }

    public Color BarColor
    {
        get => _drawable.BarColor;
        set => _drawable.BarColor = value;
    }

    public void Push(float level)
    {
        _drawable.Push(level);
        Invalidate();
    }

    public void Reset()
    {
        _drawable.Reset();
        Invalidate();
    }

    private sealed class WaveDrawable : IDrawable
    {
        private const float BarWidth = 3f;
        private const float Gap = 2f;
        private const int MaxSamples = 400;

        private readonly List<float> _levels = new();
        public Color BarColor { get; set; } = Colors.Gray;

        public void Push(float level)
        {
            _levels.Add(Math.Clamp(level, 0f, 1f));
            if (_levels.Count > MaxSamples)
                _levels.RemoveRange(0, _levels.Count - MaxSamples);
        }

        public void Reset() => _levels.Clear();

        public void Draw(ICanvas canvas, RectF rect)
        {
            if (_levels.Count == 0) return;

            canvas.FillColor = BarColor;
            var x = rect.Right - BarWidth;
            for (int i = _levels.Count - 1; i >= 0 && x >= rect.Left; i--)
            {
                var h = Math.Max(2f, _levels[i] * rect.Height);
                var y = rect.Top + (rect.Height - h) / 2f;
                canvas.FillRoundedRectangle(x, y, BarWidth, h, 1.5f);
                x -= BarWidth + Gap;
            }
        }
    }
}
