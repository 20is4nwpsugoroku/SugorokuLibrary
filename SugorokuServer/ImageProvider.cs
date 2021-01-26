using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SugorokuLibrary;

namespace SugorokuServer
{
    public static class ImageProvider
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        private static Image? CreateMapImage()
        {
            using var stream = Assembly.GetManifestResourceStream("SugorokuServer.ImageResources.viewEventsMap.png");
            return Image.Load(stream);
        }

        private static IEnumerable<Image> CreatePlayerKomaImages(int count)
        {
            var images = new List<Image>();

            for (var i = 0; i < count; i++)
            {
                using var stream =
                    Assembly.GetManifestResourceStream($"SugorokuServer.ImageResources.koma_{i + 1}.png");
                images.Add(Image.Load(stream));
            }

            return images;
        }

        private static Image? GeneratePlayerAddedMapImage(IReadOnlyList<Player> players)
        {
            var mapImage = CreateMapImage();
            if (mapImage == null) return null;

            foreach (var (komaImage, i) in CreatePlayerKomaImages(players.Count).Select((image, i) => (image, i)))
            {
                var playerData = players[i];
                var (presetX, presetY) = i switch
                {
                    0 => (0, 0),
                    1 => (55, 0),
                    2 => (0, 55),
                    3 => (55, 55),
                    _ => (22, -55)
                };

                var (x, y) = playerData.Position switch
                {
                    var pos when pos < 9 => (26 + 107 * pos, 142),
                    var pos when pos == 9 => (980, 176),
                    var pos when pos == 10 => (1038, 258),
                    var pos when pos < 21 => (999 - 108 * (pos - 11), 344),
                    var pos when pos == 21 => (0, 434),
                    var pos when pos < 30 => (22 + 108 * (pos - 22), 533),
                    var pos when pos == 30 => (930, 520),
                    _ => (0, 0)
                };

                mapImage.Mutate(ctx =>
                {
                    // resize koma image
                    komaImage.Mutate(w => { w.Resize(60, 60); });

                    ctx.DrawImage(komaImage, new Point(x + presetX, y + presetY), 1);
                });
            }

            // Generate Time data
            var collection = new FontCollection();
            using var fontStream =
                Assembly.GetManifestResourceStream("SugorokuServer.ImageResources.NotoSans-Regular.ttf");
            if (fontStream == null)
            {
                return mapImage;
            }

            var family = collection.Install(fontStream);
            var font = family.CreateFont(20);
            var now = DateTime.Now;
            mapImage.Mutate(ctx => ctx.DrawText($"Generate: {now:HH:mm:ss}", font, Color.Black, new Point(800, 680)));

            return mapImage;
        }

        private static byte[]? GenerateImageBytes(Image? img)
        {
            if (img == null) return null;

            using var ms = new MemoryStream();
            img.Save(ms, new PngEncoder());

            return ms.GetBuffer();
        }

        public static string? GenerateEncodedMapImage(IReadOnlyList<Player> players)
        {
            using var img = GeneratePlayerAddedMapImage(players);
            var bytes = GenerateImageBytes(img);
            return bytes == null ? null : Convert.ToBase64String(bytes);
        }
    }
}