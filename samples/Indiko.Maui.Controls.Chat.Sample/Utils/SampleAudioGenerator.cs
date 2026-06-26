using System.Text;

namespace Indiko.Maui.Controls.Chat.Sample.Utils;

/// <summary>
/// Generates a short, real (playable) WAV clip in memory so the sample can post voice-note
/// messages without bundling binary assets. The tone's amplitude wobbles so playback is
/// audible and the waveform has visible shape.
/// </summary>
public static class SampleAudioGenerator
{
    public static (byte[] Bytes, TimeSpan Duration) GenerateWav(int seconds = 3, double frequency = 440, int sampleRate = 22050)
    {
        var totalSamples = seconds * sampleRate;
        var dataSize = totalSamples * 2; // 16-bit mono

        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write(Encoding.ASCII.GetBytes("RIFF"));
        bw.Write(36 + dataSize);
        bw.Write(Encoding.ASCII.GetBytes("WAVE"));
        bw.Write(Encoding.ASCII.GetBytes("fmt "));
        bw.Write(16);             // PCM header size
        bw.Write((short)1);       // PCM format
        bw.Write((short)1);       // mono
        bw.Write(sampleRate);
        bw.Write(sampleRate * 2); // byte rate (mono, 2 bytes/sample)
        bw.Write((short)2);       // block align
        bw.Write((short)16);      // bits per sample
        bw.Write(Encoding.ASCII.GetBytes("data"));
        bw.Write(dataSize);

        for (int i = 0; i < totalSamples; i++)
        {
            var t = (double)i / sampleRate;
            var envelope = 0.5 * (1 - Math.Cos(2 * Math.PI * t / seconds)); // fade in/out
            var wobble = 0.3 + 0.7 * Math.Abs(Math.Sin(2 * Math.PI * t * 1.5));
            var value = Math.Sin(2 * Math.PI * frequency * t) * envelope * wobble * 0.8;
            bw.Write((short)(value * short.MaxValue));
        }

        bw.Flush();
        return (ms.ToArray(), TimeSpan.FromSeconds(seconds));
    }
}
