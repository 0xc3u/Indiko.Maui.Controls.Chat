using CoreGraphics;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// Draws a voice-note waveform from normalized (0..1) amplitude samples and tints the
/// portion up to <see cref="Progress"/> to indicate playback position.
/// </summary>
internal sealed class WaveformView : UIView
{
    private float[] _samples = [];
    private nfloat _progress;

    public WaveformView()
    {
        BackgroundColor = UIColor.Clear;
        Opaque = false;
        ContentMode = UIViewContentMode.Redraw;
    }

    public UIColor BarColor { get; set; } = UIColor.SystemGray3;
    public UIColor ProgressColor { get; set; } = UIColor.White;

    public float[] Samples
    {
        get => _samples;
        set
        {
            _samples = value ?? [];
            SetNeedsDisplay();
        }
    }

    /// <summary>Playback position in the range 0..1.</summary>
    public nfloat Progress
    {
        get => _progress;
        set
        {
            var clamped = (nfloat)Math.Clamp((double)value, 0, 1);
            if (Math.Abs(clamped - _progress) < 0.001) return;
            _progress = clamped;
            SetNeedsDisplay();
        }
    }

    public override void Draw(CGRect rect)
    {
        base.Draw(rect);
        if (_samples.Length == 0 || rect.Width <= 0 || rect.Height <= 0) return;

        using var context = UIGraphics.GetCurrentContext();

        const float barWidth = 3f;
        const float gap = 2f;
        var step = barWidth + gap;
        var maxBars = Math.Max(1, (int)(rect.Width / step));
        var count = Math.Min(_samples.Length, maxBars);
        var midY = rect.Height / 2f;
        var progressX = rect.Width * _progress;

        for (int i = 0; i < count; i++)
        {
            // Sample the source array evenly across the available bars.
            var sampleIndex = count >= _samples.Length ? i : (int)((long)i * _samples.Length / count);
            var amplitude = (nfloat)Math.Clamp(_samples[sampleIndex], 0.05f, 1f);

            var barHeight = amplitude * rect.Height;
            var x = i * step;
            var barRect = new CGRect(x, midY - barHeight / 2f, barWidth, barHeight);

            var color = (x + barWidth / 2f) <= progressX ? ProgressColor : BarColor;
            context.SetFillColor(color.CGColor);

            using var path = UIBezierPath.FromRoundedRect(barRect, barWidth / 2f);
            context.AddPath(path.CGPath);
            context.FillPath();
        }
    }
}
