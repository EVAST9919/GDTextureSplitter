using System;
using System.IO;

namespace GDTextureSplitter
{
    class Program
    {
        static void Main()
        {
            if (!Directory.Exists("Input"))
            {
                Directory.CreateDirectory("Input");
                Console.WriteLine("Nothing to convert :(");
                return;
            }

            if (!Directory.Exists("Output"))
                Directory.CreateDirectory("Output");

            var atlases = Directory.GetFiles("Input", "*.png");
            Console.WriteLine($"{atlases.Length} atlases found.");

            for (int i = 0; i < atlases.Length; i++)
            {
                Console.WriteLine();
                Console.WriteLine($"Atlas {i + 1} out of {atlases.Length}:");

                var atlas = atlases[i];
                var name = atlas[(atlas.IndexOf('\\') + 1)..atlas.IndexOf(".png")];
                Splitter.Split(name);
            }
        }
    }
}
