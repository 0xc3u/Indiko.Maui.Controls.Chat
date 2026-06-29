using System.Diagnostics;
using AudioToolbox;
using AVFoundation;
using Foundation;

namespace Indiko.Maui.Controls.Chat;

/// <summary>
/// iOS voice-note recorder (AVAudioRecorder → AAC/m4a). Used by <see cref="ChatInputView"/>.
/// The consuming app must declare <c>NSMicrophoneUsageDescription</c> in its Info.plist.
/// </summary>
internal sealed class AudioRecorderService
{
    private AVAudioRecorder _recorder;
    private NSUrl _url;
    private readonly Stopwatch _stopwatch = new();

    public bool IsRecording { get; private set; }

    public async Task<bool> StartAsync()
    {
        if (!await RequestPermissionAsync())
            return false;

        var session = AVAudioSession.SharedInstance();
        session.SetCategory(AVAudioSessionCategory.PlayAndRecord);
        session.SetActive(true);

        var path = Path.Combine(Path.GetTempPath(), $"voice_{Guid.NewGuid():N}.m4a");
        _url = NSUrl.FromFilename(path);

        var settings = new AudioSettings
        {
            Format = AudioFormatType.MPEG4AAC,
            SampleRate = 44100,
            NumberChannels = 1,
            AudioQuality = AVAudioQuality.High,
        };

        try
        {
            _recorder = AVAudioRecorder.Create(_url, settings, out var error);
            if (_recorder == null || error != null)
                return false;

            _recorder.PrepareToRecord();
            if (!_recorder.Record())
                return false;

            _stopwatch.Restart();
            IsRecording = true;
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AudioRecorderService.StartAsync: {ex.Message}");
            _recorder?.Dispose();
            _recorder = null;
            IsRecording = false;
            return false;
        }
    }

    public async Task<(byte[] Bytes, TimeSpan Duration)?> StopAsync()
    {
        if (!IsRecording || _recorder == null)
            return null;

        IsRecording = false;
        _stopwatch.Stop();
        _recorder.Stop();
        var duration = _stopwatch.Elapsed;
        var path = _url?.Path;
        _recorder.Dispose();
        _recorder = null;
        try { AVAudioSession.SharedInstance().SetActive(false); } catch { /* ignore */ }

        if (path == null || !File.Exists(path))
            return null;

        var bytes = await File.ReadAllBytesAsync(path);
        try { File.Delete(path); } catch { /* ignore */ }
        return (bytes, duration);
    }

    public void Cancel()
    {
        if (_recorder != null)
        {
            try { _recorder.Stop(); } catch { /* ignore */ }
            _recorder.Dispose();
            _recorder = null;
        }
        IsRecording = false;
        _stopwatch.Reset();

        var path = _url?.Path;
        if (path != null && File.Exists(path))
        {
            try { File.Delete(path); } catch { /* ignore */ }
        }
        try { AVAudioSession.SharedInstance().SetActive(false); } catch { /* ignore */ }
    }

    private static Task<bool> RequestPermissionAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        AVAudioSession.SharedInstance().RequestRecordPermission(granted => tcs.TrySetResult(granted));
        return tcs.Task;
    }
}
