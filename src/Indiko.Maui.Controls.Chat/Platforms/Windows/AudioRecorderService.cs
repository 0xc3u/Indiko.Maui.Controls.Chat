using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Maui.ApplicationModel;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;

namespace Indiko.Maui.Controls.Chat;

/// <summary>
/// COM interface used to read the raw bytes of a captured <see cref="AudioBuffer"/>'s memory
/// reference for live level metering.
/// </summary>
[ComImport]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal unsafe interface IMemoryBufferByteAccess
{
    void GetBuffer(out byte* buffer, out uint capacity);
}

/// <summary>
/// Windows voice-note recorder (AudioGraph → m4a). Used by <see cref="ChatInputView"/>.
/// A single graph feeds the file output node (recording) and a frame output node (live level
/// metering for the waveform). The consuming app must declare the <c>microphone</c> device
/// capability in its Package.appxmanifest.
/// </summary>
internal sealed class AudioRecorderService
{
    private AudioGraph _graph;
    private AudioDeviceInputNode _input;
    private AudioFileOutputNode _fileOutput;
    private AudioFrameOutputNode _frameOutput;
    private string _path;
    private readonly Stopwatch _stopwatch = new();
    private float _level;

    public bool IsRecording { get; private set; }

    public async Task<bool> StartAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
            return false;

        try
        {
            var settings = new AudioGraphSettings(AudioRenderCategory.Media);
            var createResult = await AudioGraph.CreateAsync(settings);
            if (createResult.Status != AudioGraphCreationStatus.Success)
                return false;
            _graph = createResult.Graph;

            var inputResult = await _graph.CreateDeviceInputNodeAsync(MediaCategory.Media);
            if (inputResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                CleanupGraph();
                return false;
            }
            _input = inputResult.DeviceInputNode;

            _path = Path.Combine(Path.GetTempPath(), $"voice_{Guid.NewGuid():N}.m4a");
            File.Create(_path).Dispose();
            var file = await StorageFile.GetFileFromPathAsync(_path);

            var profile = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Medium);
            var fileResult = await _graph.CreateFileOutputNodeAsync(file, profile);
            if (fileResult.Status != AudioFileNodeCreationStatus.Success)
            {
                CleanupGraph();
                DeleteTempFile();
                return false;
            }
            _fileOutput = fileResult.FileOutputNode;
            _input.AddOutgoingConnection(_fileOutput);

            // Level metering: a frame output node lets us read the captured PCM without
            // disturbing the file recording. GetLevel() drains the latest frame.
            try
            {
                var encoding = _input.EncodingProperties ?? AudioEncodingProperties.CreatePcm(44100, 1, 32);
                _frameOutput = _graph.CreateFrameOutputNode(encoding);
                _input.AddOutgoingConnection(_frameOutput);
            }
            catch (Exception ex)
            {
                // Level metering is best-effort; recording continues without it.
                Debug.WriteLine($"AudioRecorderService: level metering unavailable: {ex.Message}");
                _frameOutput = null;
            }

            _graph.Start();
            _stopwatch.Restart();
            IsRecording = true;
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AudioRecorderService.StartAsync: {ex.Message}");
            CleanupGraph();
            DeleteTempFile();
            IsRecording = false;
            return false;
        }
    }

    public async Task<(byte[] Bytes, TimeSpan Duration)?> StopAsync()
    {
        if (!IsRecording || _graph == null)
            return null;

        IsRecording = false;
        _stopwatch.Stop();
        var duration = _stopwatch.Elapsed;

        try { _graph.Stop(); } catch { /* already stopped */ }
        CleanupGraph();

        if (_path == null || !File.Exists(_path))
            return null;

        var bytes = await File.ReadAllBytesAsync(_path);
        DeleteTempFile();
        return (bytes, duration);
    }

    /// <summary>
    /// Current normalized (0..1) input level for the live waveform. Reads the latest captured
    /// audio frame (null when no new frame arrived since the previous call) and computes the RMS.
    /// </summary>
    public float GetLevel()
    {
        if (!IsRecording || _frameOutput == null)
            return 0f;

        try
        {
            using var frame = _frameOutput.GetFrame();
            if (frame == null)
            {
                // No new frame yet — decay so the bars fall naturally.
                _level *= 0.85f;
                return Math.Max(0f, _level);
            }

            using var buffer = frame.LockBuffer(Windows.Media.AudioBufferAccessMode.Read);
            using var reference = buffer.CreateReference();

            // The frame encoding is 32-bit float PCM (see StartAsync); compute RMS over the
            // samples exposed through the memory buffer's byte-access COM interface.
            double sumSquares = 0;
            uint sampleCount = 0;
            unsafe
            {
                var byteAccess = (IMemoryBufferByteAccess)(object)reference;
                byte* data;
                byteAccess.GetBuffer(out data, out var capacity);
                var floatData = (float*)data;
                sampleCount = capacity / 4;
                for (uint i = 0; i < sampleCount; i++)
                {
                    var sample = floatData[i];
                    sumSquares += sample * sample;
                }
            }

            var rms = sampleCount > 0 ? Math.Sqrt(sumSquares / sampleCount) : 0;
            var level = (float)Math.Min(1f, rms * 3.5);
            _level = Math.Max(level, _level * 0.85f);
            return _level;
        }
        catch
        {
            // Metering failure must never disturb recording.
            return 0f;
        }
    }

    public void Cancel()
    {
        if (_graph != null)
        {
            try { _graph.Stop(); } catch { /* ignore */ }
        }
        CleanupGraph();
        IsRecording = false;
        _stopwatch.Reset();
        _level = 0f;
        DeleteTempFile();
    }

    private void CleanupGraph()
    {
        try { _frameOutput?.Dispose(); } catch { /* ignore */ }
        try { _fileOutput?.Dispose(); } catch { /* ignore */ }
        try { _input?.Dispose(); } catch { /* ignore */ }
        try { _graph?.Dispose(); } catch { /* ignore */ }
        _frameOutput = null;
        _fileOutput = null;
        _input = null;
        _graph = null;
    }

    private void DeleteTempFile()
    {
        if (_path != null && File.Exists(_path))
        {
            try { File.Delete(_path); } catch { /* ignore */ }
        }
    }
}
