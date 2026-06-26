namespace Indiko.Maui.Controls.Chat.Sample.Utils;

/// <summary>
/// Generates a small uncompressed 24-bit BMP in memory so the sample can post image messages
/// (e.g. captioned images) without bundling assets or going through the photo picker.
/// Both iOS (ImageIO) and Android (BitmapFactory) decode BMP from the byte content.
/// </summary>
public static class SampleImageGenerator
{
    public static byte[] GenerateBmp(int width = 240, int height = 160, byte r = 0x4B, byte g = 0x00, byte b = 0xDF)
    {
        var rowSize = (width * 3 + 3) & ~3;       // rows padded to a 4-byte boundary
        var pixelDataSize = rowSize * height;
        var fileSize = 54 + pixelDataSize;

        var bytes = new byte[fileSize];

        // BITMAPFILEHEADER (14 bytes)
        bytes[0] = (byte)'B'; bytes[1] = (byte)'M';
        WriteInt(bytes, 2, fileSize);
        WriteInt(bytes, 10, 54);                  // pixel data offset

        // BITMAPINFOHEADER (40 bytes)
        WriteInt(bytes, 14, 40);
        WriteInt(bytes, 18, width);
        WriteInt(bytes, 22, height);
        bytes[26] = 1;                            // planes
        bytes[28] = 24;                           // bits per pixel
        WriteInt(bytes, 34, pixelDataSize);

        // Pixel data: a vertical gradient over the base colour so it's clearly an image.
        for (int y = 0; y < height; y++)
        {
            var rowStart = 54 + y * rowSize;
            var shade = (float)y / height;        // 0 (bottom) .. 1 (top, BMP is bottom-up)
            for (int x = 0; x < width; x++)
            {
                var i = rowStart + x * 3;
                bytes[i] = (byte)(b * (0.4f + 0.6f * shade));   // BMP stores BGR
                bytes[i + 1] = (byte)(g * (0.4f + 0.6f * shade));
                bytes[i + 2] = (byte)(r * (0.4f + 0.6f * shade));
            }
        }

        return bytes;
    }

    private static void WriteInt(byte[] buffer, int offset, int value)
    {
        buffer[offset] = (byte)(value & 0xFF);
        buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
        buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
    }
}
