using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GDTextureSplitter
{
    public static class Splitter
    {
        public static void Split(string filename)
        {
            if (!File.Exists($"Input/{filename}.plist"))
            {
                Console.WriteLine($"Can't find .plist file for {filename}");
            }

            string data = File.ReadAllText($"Input/{filename}.plist");
            Bitmap atlas = Image.FromFile($"Input/{filename}.png") as Bitmap;

            var textures = new List<GDTexture>();

            Console.WriteLine($"Parsing {filename}.plist file...");

            while (data.Contains("textureRect"))
            {
                if (!data.Contains(".png"))
                    break;

                var nameEndIndex = data.IndexOf(".png");
                var nameStartIndex = data.LastIndexOf('>', nameEndIndex) + 1;
                string name = data[nameStartIndex..nameEndIndex];

                if (!data.Contains("spriteSize"))
                    break;

                var sizeStartIndex = data.IndexOf("spriteSize");
                var widthIndex = data.IndexOf('{', sizeStartIndex);
                var widthString = data.Substring(widthIndex + 1, data.IndexOf(',', widthIndex) - widthIndex - 1);
                int width = int.Parse(widthString);

                var heightIndex = data.IndexOf(',', widthIndex);
                var heightString = data.Substring(heightIndex + 1, data.IndexOf('}', heightIndex) - heightIndex - 1);
                int height = int.Parse(heightString);

                if (!data.Contains("textureRect"))
                    break;

                var rectStartIndex = data.IndexOf("textureRect");
                var xIndex = data.IndexOf("{{", rectStartIndex);
                var xString = data.Substring(xIndex + 2, data.IndexOf(',', xIndex) - xIndex - 2);
                int x = int.Parse(xString);

                var yIndex = data.IndexOf(',', xIndex);
                var yString = data.Substring(yIndex + 1, data.IndexOf('}', yIndex) - yIndex - 1);
                int y = int.Parse(yString);

                if (!data.Contains("textureRotated"))
                    break;

                var rotatedIndex = data.IndexOf("textureRotated");
                var rotatedStartIndex = data.IndexOf('<', data.IndexOf("</key>", rotatedIndex) + 1);
                var rotatedValue = data.Substring(rotatedStartIndex + 1, data.IndexOf("/>", rotatedStartIndex + 1) - rotatedStartIndex - 1);
                bool rotated = bool.Parse(rotatedValue);

                data = data[rotatedStartIndex..];

                textures.Add(new GDTexture
                {
                    Name = name,
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    Rotated = rotated
                });
            }

            Console.WriteLine($"{textures.Count} textures found!");

            if (!Directory.Exists($"Output/{filename}"))
                Directory.CreateDirectory($"Output/{filename}");

            for (int i = 0; i < textures.Count; i++)
            {
                Console.Write($"\rSaving texture: {i + 1} out of {textures.Count}");

                var t = textures[i];

                var cropped = atlas.Clone(new RectangleF(t.X, t.Y, t.Rotated ? t.Height : t.Width, t.Rotated ? t.Width : t.Height), atlas.PixelFormat);

                if (t.Rotated)
                    cropped.RotateFlip(RotateFlipType.Rotate270FlipNone);

                cropped.Save($"Output/{filename}/{t.Name}.png", ImageFormat.Png);
                cropped.Dispose();
            }

            Console.WriteLine();
            Console.WriteLine($"Splitting {filename} Complete!");
        }

        private class GDTexture
        {
            public string Name { get; set; }

            public int X { get; set; }

            public int Y { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public bool Rotated { get; set; }
        }
    }
}
