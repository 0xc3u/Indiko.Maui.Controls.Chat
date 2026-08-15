using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using WindowsColor = Windows.UI.Color;
using MauiColor = Microsoft.Maui.Graphics.Color;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// Shared helpers for the Windows chat implementation: the attached property that links a
/// <see cref="ListView"/> to its owning <see cref="ChatViewHandler"/> (so message templates can
/// raise the virtual view's commands on interaction), color/bitmap conversions, link detection
/// for the message RichTextBlocks and visual-tree utilities.
/// </summary>
internal static class ChatViewState
{
    // ---- Attached handler -------------------------------------------------------------------

    /// <summary>
    /// Attached to the chat's <see cref="ListView"/> by the handler. Template event handlers
    /// resolve it by walking up the visual tree, then raise commands on the <see cref="ChatView"/>.
    /// </summary>
    public static readonly DependencyProperty HandlerProperty = DependencyProperty.RegisterAttached(
        "Handler",
        typeof(ChatViewHandler),
        typeof(ChatViewState),
        new PropertyMetadata(null));

    public static void SetHandler(DependencyObject element, ChatViewHandler value) => element.SetValue(HandlerProperty, value);

    public static ChatViewHandler GetHandler(DependencyObject element) => (ChatViewHandler)element.GetValue(HandlerProperty);

    // ---- Color / bitmap conversions --------------------------------------------------------

    public static WindowsColor ToWindowsColor(MauiColor color)
        => WindowsColor.FromArgb(
            (byte)(color.Alpha * 255),
            (byte)(color.Red * 255),
            (byte)(color.Green * 255),
            (byte)(color.Blue * 255));

    public static SolidColorBrush Brush(MauiColor color) => new(ToWindowsColor(color));

    public static WindowsColor WithAlpha(WindowsColor color, float alpha)
        => WindowsColor.FromArgb((byte)(alpha * 255), color.R, color.G, color.B);

    public static async Task<BitmapImage> BitmapFromBytesAsync(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return null;

        try
        {
            using var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    // ---- Media temp files -------------------------------------------------------------------

    /// <summary>
    /// Writes message binary content (video/audio) to a temp file keyed by the message id so it
    /// is written at most once per message, then returns the file path for the media player.
    /// </summary>
    public static string EnsureTempFile(string fileName, byte[] bytes)
    {
        if (string.IsNullOrEmpty(fileName) || bytes == null || bytes.Length == 0)
            return null;

        try
        {
            var path = Path.Combine(Path.GetTempPath(), fileName);
            if (!File.Exists(path))
                File.WriteAllBytes(path, bytes);
            return path;
        }
        catch
        {
            return null;
        }
    }

    // ---- Link detection (URLs / www / e-mails) ------------------------------------------------

    private static readonly Regex LinkRegex = new(
        @"(https?://[^\s<>""]+)|(www\.[^\s<>""]+)|([A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,})",
        RegexOptions.Compiled);

    /// <summary>
    /// Populates a <see cref="RichTextBlock"/> with the message text, rendering detected URLs,
    /// www-links and e-mail addresses as tappable hyperlinks (opening the default browser/mail
    /// app) when <paramref name="detectLinks"/> is true.
    /// </summary>
    public static void SetMessageText(RichTextBlock block, string text, double fontSize, WindowsColor textColor, WindowsColor linkColor, bool detectLinks)
    {
        block.Blocks.Clear();
        var paragraph = new Paragraph();

        if (string.IsNullOrEmpty(text))
        {
            block.Blocks.Add(paragraph);
            return;
        }

        if (!detectLinks)
        {
            paragraph.Inlines.Add(new Run { Text = text, FontSize = fontSize, Foreground = new SolidColorBrush(textColor) });
            block.Blocks.Add(paragraph);
            return;
        }

        var lastIndex = 0;
        foreach (Match match in LinkRegex.Matches(text))
        {
            if (match.Index > lastIndex)
            {
                paragraph.Inlines.Add(new Run
                {
                    Text = text[lastIndex..match.Index],
                    FontSize = fontSize,
                    Foreground = new SolidColorBrush(textColor),
                });
            }

            var target = NormalizeLink(match.Value);
            var hyperlink = new Hyperlink
            {
                Foreground = new SolidColorBrush(linkColor),
            };
            hyperlink.Inlines.Add(new Run { Text = match.Value, FontSize = fontSize });
            var url = target;
            hyperlink.Click += (s, e) =>
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    _ = Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(uri);
            };
            paragraph.Inlines.Add(hyperlink);

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < text.Length)
        {
            paragraph.Inlines.Add(new Run
            {
                Text = text[lastIndex..],
                FontSize = fontSize,
                Foreground = new SolidColorBrush(textColor),
            });
        }

        block.Blocks.Add(paragraph);
    }

    private static string NormalizeLink(string value)
    {
        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            value.Contains('@'))
        {
            return value.Contains('@') && !value.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                ? $"mailto:{value}"
                : value;
        }

        return $"http://{value}";
    }

    // ---- Visual tree helpers ------------------------------------------------------------------

    public static T FindAncestor<T>(DependencyObject reference) where T : class
    {
        var current = reference;
        while (current != null)
        {
            if (current is T match)
                return match;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    /// <summary>Walks up until it finds an element whose <c>Name</c> matches.</summary>
    public static FrameworkElement FindNamedAncestor(DependencyObject reference, string name)
    {
        var current = reference;
        while (current != null)
        {
            if (current is FrameworkElement fe && fe.Name == name)
                return fe;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    /// <summary>Finds the inner <see cref="ScrollViewer"/> of a <see cref="ListViewBase"/>.</summary>
    public static ScrollViewer GetScrollViewer(DependencyObject reference)
    {
        var count = VisualTreeHelper.GetChildrenCount(reference);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(reference, i);
            if (child is ScrollViewer scrollViewer)
                return scrollViewer;

            var result = GetScrollViewer(child);
            if (result != null)
                return result;
        }
        return null;
    }
}
