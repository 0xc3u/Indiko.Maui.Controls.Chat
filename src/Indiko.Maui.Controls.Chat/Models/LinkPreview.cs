namespace Indiko.Maui.Controls.Chat.Models;

/// <summary>
/// Data for a link-preview ("unfurl") card rendered under a text message. The control only renders
/// this — the consuming app is responsible for fetching the URL's metadata (e.g. OpenGraph tags)
/// and the optional thumbnail bytes, in keeping with the library's no-networking design.
/// </summary>
public class LinkPreview
{
    /// <summary>The target URL opened when the card is tapped.</summary>
    public string Url { get; set; }

    /// <summary>Headline shown on the card (e.g. the page's og:title).</summary>
    public string Title { get; set; }

    /// <summary>Short description shown under the title (e.g. the page's og:description).</summary>
    public string Description { get; set; }

    /// <summary>Site/domain label shown above the title (e.g. "github.com").</summary>
    public string SiteName { get; set; }

    /// <summary>Optional thumbnail image bytes shown at the top of the card.</summary>
    public byte[] ImageBytes { get; set; }
}
