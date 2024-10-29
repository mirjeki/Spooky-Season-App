using SkiaSharp;
using SkiaSharp.Views.Maui;
using Microsoft.Maui.Storage;
using System.Reflection;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace SpookySeason
{
    public class FilmImageService
    {
        public async Task<string> CreateFilmImageWithText(Film film, string filmImageFilePath)
        {
            try
            {
                var convertedFilmTitle = film.Title.Replace(' ', '_').ToLower();

                var originalImage = await LoadBitmapFromImageSource(filmImageFilePath);
                // Create a new image with the film data
                using (var surface = SKSurface.Create(new SKImageInfo(originalImage.Width, originalImage.Height)))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear();

                    // Draw the original image
                    canvas.DrawBitmap(originalImage, SKRect.Create(originalImage.Width, originalImage.Height));

                    // Set up the paint for text drawing
                    var textPaint = new SKPaint
                    {
                        Color = SKColors.White,
                        TextSize = 64,
                        IsAntialias = true,
                        Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                    };

                    // Draw the film title
                    canvas.DrawText(film.Title, 50, 100, textPaint);

                    // Draw the score
                    canvas.DrawText($"Rating: {film.Rating}/10", 50, 200, textPaint);

                    // Draw the comments
                    canvas.DrawText($"Comments: {film.Comments}", 50, 400, textPaint);

                    // Save the image to a file
                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        var filePath = Path.Combine(FileSystem.CacheDirectory, $"{convertedFilmTitle}_review.png");
                        using (var fileStream = File.OpenWrite(filePath))
                        {
                            data.SaveTo(fileStream);
                        }

                        return filePath; // Return the path of the saved image
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                throw;
            }
        }

        public async Task<SKBitmap> LoadBitmapFromImageSource(string imageSource)
        {
            using (var stream = await GetStreamFromImageSource(imageSource))
            {
                if (stream == null)
                {
                    throw new Exception("Unable to get stream from ImageSource.");
                }

                // Read the stream into a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    byte[] imageData = memoryStream.ToArray();

                    // Create SKBitmap from byte array
                    using (var skStream = new SKMemoryStream(imageData))
                    {
                        return SKBitmap.Decode(skStream);
                    }
                }
            }
        }

        private async Task<Stream> GetStreamFromImageSource(string imageSource)
        {
            return File.OpenRead(imageSource + ".jpg");
        }
    }
}
