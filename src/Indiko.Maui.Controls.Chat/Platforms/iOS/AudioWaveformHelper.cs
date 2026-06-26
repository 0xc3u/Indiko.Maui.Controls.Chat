namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

internal static class AudioWaveformHelper
{
    /// <summary>
    /// Produces a stable pseudo-waveform from a seed (e.g. the message id) so a clip without
    /// supplied samples still renders varied, consistent bars instead of a flat line.
    /// </summary>
    public static float[] GenerateFallback(string seed, int bars = 40)
    {
        var samples = new float[bars];
        // Deterministic LCG seeded from the string hash — same message → same waveform.
        ulong state = 1469598103934665603UL;
        foreach (var ch in seed ?? string.Empty)
        {
            state = (state ^ ch) * 1099511628211UL;
        }
        if (state == 0) state = 0x9E3779B97F4A7C15UL;

        for (int i = 0; i < bars; i++)
        {
            state = state * 6364136223846793005UL + 1442695040888963407UL;
            var unit = ((state >> 33) & 0xFFFF) / 65535f; // 0..1
            samples[i] = 0.2f + unit * 0.8f;              // keep bars visible (0.2..1.0)
        }
        return samples;
    }

    /// <summary>Formats a duration as m:ss (e.g. 0:07, 1:42).</summary>
    public static string Format(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero) duration = TimeSpan.Zero;
        return $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}";
    }
}
