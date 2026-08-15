using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using WindowsColor = Windows.UI.Color;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// Draws a voice-note waveform from normalized (0..1) amplitude samples as rounded bars and
/// tints the portion up to the playback progress. Mirrors the Android/iOS waveform views.
/// </summary>
internal sealed class WaveformView : Grid
{
    private const float BarWidth = 3f;
    private const float Gap = 2f;
    private const float Step = BarWidth + Gap;

    private float[] _samples = [];
    private float _progress;
    private WindowsColor _barColor = WindowsColor.FromArgb(255, 190, 190, 190);
    private WindowsColor _progressColor = WindowsColor.FromArgb(255, 255, 255, 255);
    private readonly List<Rectangle> _bars = new();
    private SolidColorBrush _barBrush = new(WindowsColor.FromArgb(255, 190, 190, 190));
    private SolidColorBrush _progressBrush = new(WindowsColor.FromArgb(255, 255, 255, 255));

    public WaveformView()
    {
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Stretch;
        IsHitTestVisible = true;
        Background = new SolidColorBrush(WindowsColor.FromArgb(0, 0, 0, 0));
        SizeChanged += (s, e) => Rebuild();
    }

    public void SetColors(WindowsColor barColor, WindowsColor progressColor)
    {
        _barColor = barColor;
        _progressColor = progressColor;
        Rebuild();
    }

    public void SetSamples(float[] samples)
    {
        _samples = samples ?? [];
        Rebuild();
    }

    /// <summary>Playback position in the range 0..1.</summary>
    public void SetProgress(float progress)
    {
        var clamped = Math.Clamp(progress, 0f, 1f);
        if (Math.Abs(clamped - _progress) < 0.001f)
            return;
        _progress = clamped;
        UpdateFills();
    }

    private void Rebuild()
    {
        Children.Clear();
        _bars.Clear();

        if (_samples.Length == 0 || ActualWidth <= 0 || ActualHeight <= 0)
            return;

        _barBrush = new SolidColorBrush(_barColor);
        _progressBrush = new SolidColorBrush(_progressColor);

        var maxBars = Math.Max(1, (int)(ActualWidth / Step));
        var count = Math.Min(_samples.Length, maxBars);
        var midY = ActualHeight / 2f;

        for (int i = 0; i < count; i++)
        {
            var sampleIndex = count >= _samples.Length ? i : (int)((long)i * _samples.Length / count);
            var amplitude = Math.Clamp(_samples[sampleIndex], 0.05f, 1f);
            var barHeight = amplitude * ActualHeight;

            var bar = new Rectangle
            {
                Width = BarWidth,
                Height = Math.Max(2, barHeight),
                RadiusX = BarWidth / 2f,
                RadiusY = BarWidth / 2f,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(i * Step, 0, 0, 0),
                Fill = new SolidColorBrush(_barColor),
                IsHitTestVisible = false,
            };
            _bars.Add(bar);
            Children.Add(bar);
        }

        UpdateFills();
    }

    private void UpdateFills()
    {
        var playedCount = (int)(_bars.Count * _progress);
        for (int i = 0; i < _bars.Count; i++)
        {
            var fill = i < playedCount ? _progressBrush : _barBrush;
            if (!ReferenceEquals(_bars[i].Fill, fill))
                _bars[i].Fill = fill;
        }
    }
}
