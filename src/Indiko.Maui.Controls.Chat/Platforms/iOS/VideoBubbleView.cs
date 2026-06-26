using AVFoundation;
using AVKit;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// Inline video bubble that shows a blurred poster (first frame) with a central play button
/// and only begins playback when the user taps it — instead of auto-playing on scroll.
/// </summary>
internal sealed class VideoBubbleView : UIView
{
    private readonly AVPlayer _player = new();
    private readonly AVPlayerViewController _playerController = new();
    private readonly UIImageView _posterView;
    private readonly UIVisualEffectView _blurView;
    private readonly UIButton _playButton;

    public VideoBubbleView()
    {
        ClipsToBounds = true;
        Layer.CornerRadius = 12;
        BackgroundColor = UIColor.Black;

        _playerController.Player = _player;
        _playerController.ShowsPlaybackControls = true;
        var playerView = _playerController.View;
        playerView.TranslatesAutoresizingMaskIntoConstraints = false;
        playerView.BackgroundColor = UIColor.Black;
        AddSubview(playerView);

        _posterView = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            ClipsToBounds = true,
            BackgroundColor = UIColor.Black
        };
        AddSubview(_posterView);

        _blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark))
        {
            TranslatesAutoresizingMaskIntoConstraints = false
        };
        AddSubview(_blurView);

        _playButton = new UIButton(UIButtonType.System)
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            TintColor = UIColor.White,
            BackgroundColor = UIColor.FromWhiteAlpha(0f, 0.35f)
        };
        _playButton.Layer.CornerRadius = 28;
        _playButton.SetImage(UIImage.GetSystemImage("play.fill")?.ApplyTintColor(UIColor.White), UIControlState.Normal);
        AddSubview(_playButton);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            playerView.TopAnchor.ConstraintEqualTo(TopAnchor),
            playerView.BottomAnchor.ConstraintEqualTo(BottomAnchor),
            playerView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            playerView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),

            _posterView.TopAnchor.ConstraintEqualTo(TopAnchor),
            _posterView.BottomAnchor.ConstraintEqualTo(BottomAnchor),
            _posterView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            _posterView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),

            _blurView.TopAnchor.ConstraintEqualTo(TopAnchor),
            _blurView.BottomAnchor.ConstraintEqualTo(BottomAnchor),
            _blurView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            _blurView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),

            _playButton.CenterXAnchor.ConstraintEqualTo(CenterXAnchor),
            _playButton.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
            _playButton.WidthAnchor.ConstraintEqualTo(56),
            _playButton.HeightAnchor.ConstraintEqualTo(56),
        });

        _playButton.TouchUpInside += (_, _) => StartPlayback();
        _blurView.ContentView.UserInteractionEnabled = true;
        _blurView.ContentView.AddGestureRecognizer(new UITapGestureRecognizer(StartPlayback));
    }

    public void Configure(string filePath)
    {
        ResetToPoster();

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            _posterView.Image = null;
            return;
        }

        var asset = AVAsset.FromUrl(NSUrl.FromFilename(filePath));
        if (asset == null) return;

        _player.ReplaceCurrentItemWithPlayerItem(new AVPlayerItem(asset));
        GeneratePoster(asset);
    }

    /// <summary>Pauses and restores the poster overlay — called on cell reuse.</summary>
    public void ResetToPoster()
    {
        _player.Pause();
        _player.Seek(CMTime.Zero);
        _posterView.Hidden = false;
        _blurView.Hidden = false;
        _playButton.Hidden = false;
    }

    private void StartPlayback()
    {
        _posterView.Hidden = true;
        _blurView.Hidden = true;
        _playButton.Hidden = true;
        _player.Play();
    }

    private void GeneratePoster(AVAsset asset)
    {
        // Extract the first frame off the main thread; AVAssetImageGenerator can be slow.
        Task.Run(() =>
        {
            try
            {
                using var generator = new AVAssetImageGenerator(asset)
                {
                    AppliesPreferredTrackTransform = true
                };
                var cgImage = generator.CopyCGImageAtTime(CMTime.Zero, out _, out _);
                if (cgImage != null)
                {
                    var poster = new UIImage(cgImage);
                    InvokeOnMainThread(() => _posterView.Image = poster);
                }
            }
            catch
            {
                // No poster — the dark blur + play button still indicate a tappable video.
            }
        });
    }
}
