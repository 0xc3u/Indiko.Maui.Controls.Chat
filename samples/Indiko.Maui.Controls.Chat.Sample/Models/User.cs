namespace Indiko.Maui.Controls.Chat.Sample.Models;

internal class User
{
    public string Name { get; set; }
    public string Initials { get; set; }
    public byte[] Avatar { get; set; }
    public bool IsOwnMessage { get; set; }
}