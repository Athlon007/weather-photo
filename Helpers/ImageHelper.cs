using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Athlon.WeatherPhoto.Helpers;

public static class ImageHelper
{
    public static Stream AddTextToImage(Stream imageStream, params (string text, (float x, float y) position, int fontSize, string colorHex)[] texts)
    {
        var memoryStream = new MemoryStream();

        using var image = Image.Load<Rgba32>(imageStream);

        var textOptions = new DrawingOptions {};

        foreach (var (text, (x, y), fontSize, colorHex) in texts)
        {
            var font = SystemFonts.CreateFont("Verdana", fontSize);
            var color = Color.Parse(colorHex);

            image.Mutate(ctx => ctx.DrawText(textOptions, text, font, color, new PointF(x, y)));
        }

        image.SaveAsPng(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public static Stream DownscaleImage(Stream imageStream, int width, int height)
    {
        var memoryStream = new MemoryStream();

        using var image = Image.Load<Rgba32>(imageStream);

        image.Mutate(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Max
        }));

        image.SaveAsPng(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }
}