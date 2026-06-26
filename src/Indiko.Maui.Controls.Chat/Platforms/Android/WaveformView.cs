using Android.Content;
using Android.Graphics;
using AView = Android.Views.View;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// Draws a voice-note waveform from normalized (0..1) amplitude samples and tints the
/// portion up to the playback progress.
/// </summary>
public sealed class WaveformView : AView
{
    private float[] _samples = [];
    private float _progress;
    private readonly Paint _barPaint = new(PaintFlags.AntiAlias);
    private readonly Paint _progressPaint = new(PaintFlags.AntiAlias);

    public WaveformView(Context context) : base(context)
    {
        _barPaint.Color = Color.LightGray;
        _progressPaint.Color = Color.White;
    }

    public void SetColors(Color barColor, Color progressColor)
    {
        _barPaint.Color = barColor;
        _progressPaint.Color = progressColor;
        Invalidate();
    }

    public void SetSamples(float[] samples)
    {
        _samples = samples ?? [];
        Invalidate();
    }

    /// <summary>Playback position in the range 0..1.</summary>
    public void SetProgress(float progress)
    {
        var clamped = Math.Clamp(progress, 0f, 1f);
        if (Math.Abs(clamped - _progress) < 0.001f) return;
        _progress = clamped;
        Invalidate();
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);
        if (_samples.Length == 0 || Width <= 0 || Height <= 0) return;

        var density = Resources.DisplayMetrics.Density;
        var barWidth = 3f * density;
        var gap = 2f * density;
        var step = barWidth + gap;
        var maxBars = Math.Max(1, (int)(Width / step));
        var count = Math.Min(_samples.Length, maxBars);
        var midY = Height / 2f;
        var progressX = Width * _progress;
        var radius = barWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            var sampleIndex = count >= _samples.Length ? i : (int)((long)i * _samples.Length / count);
            var amplitude = Math.Clamp(_samples[sampleIndex], 0.05f, 1f);
            var barHeight = amplitude * Height;
            var x = i * step;
            var center = x + barWidth / 2f;
            var paint = center <= progressX ? _progressPaint : _barPaint;
            canvas.DrawRoundRect(x, midY - barHeight / 2f, x + barWidth, midY + barHeight / 2f, radius, radius, paint);
        }
    }
}
