using System.Diagnostics;
using Android.Media;
using Microsoft.Maui.ApplicationModel;

namespace Indiko.Maui.Controls.Chat;

/// <summary>
/// Android voice-note recorder (MediaRecorder → AAC/m4a). Used by <see cref="ChatInputView"/>.
/// The consuming app must declare the <c>RECORD_AUDIO</c> permission in its AndroidManifest.
/// </summary>
internal sealed class AudioRecorderService
{
    private MediaRecorder _recorder;
    private string _path;
    private readonly Stopwatch _stopwatch = new();

    public bool IsRecording { get; private set; }

    public async Task<bool> StartAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
            return false;

        var context = global::Android.App.Application.Context;
        _path = Path.Combine(Path.GetTempPath(), $"voice_{Guid.NewGuid():N}.m4a");

        try
        {
            _recorder = OperatingSystem.IsAndroidVersionAtLeast(31)
                ? new MediaRecorder(context)
                : new MediaRecorder();

            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _recorder.SetOutputFile(_path);
            _recorder.Prepare();
            _recorder.Start();
            _stopwatch.Restart();
            IsRecording = true;
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AudioRecorderService.StartAsync: {ex.Message}");
            try { _recorder?.Release(); _recorder?.Dispose(); } catch { /* ignore */ }
            _recorder = null;
            IsRecording = false;
            return false;
        }
    }

    public Task<(byte[] Bytes, TimeSpan Duration)?> StopAsync()
    {
        if (!IsRecording || _recorder == null)
            return Task.FromResult<(byte[] Bytes, TimeSpan Duration)?>(null);

        IsRecording = false;
        _stopwatch.Stop();
        try { _recorder.Stop(); } catch { /* too short / not started */ }
        _recorder.Release();
        _recorder.Dispose();
        _recorder = null;
        var duration = _stopwatch.Elapsed;

        if (_path == null || !File.Exists(_path))
            return Task.FromResult<(byte[] Bytes, TimeSpan Duration)?>(null);

        var bytes = File.ReadAllBytes(_path);
        try { File.Delete(_path); } catch { /* ignore */ }
        return Task.FromResult<(byte[] Bytes, TimeSpan Duration)?>((bytes, duration));
    }

    /// <summary>Current normalized (0..1) input level for the live waveform.</summary>
    public float GetLevel()
    {
        if (!IsRecording || _recorder == null)
            return 0f;
        try
        {
            // MaxAmplitude is 0..32767 (max since the previous call); scale for visual sensitivity.
            return Math.Min(1f, _recorder.MaxAmplitude / 16000f);
        }
        catch { return 0f; }
    }

    public void Cancel()
    {
        if (_recorder != null)
        {
            try { _recorder.Stop(); } catch { /* ignore */ }
            _recorder.Release();
            _recorder.Dispose();
            _recorder = null;
        }
        IsRecording = false;
        _stopwatch.Reset();

        if (_path != null && File.Exists(_path))
        {
            try { File.Delete(_path); } catch { /* ignore */ }
        }
    }
}
