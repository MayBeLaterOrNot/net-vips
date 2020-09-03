namespace NetVips.Samples
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// See: https://github.com/kleisauke/net-vips/issues/96
    /// </summary>
    public class RandomCropper : ISample
    {
        public string Name => "Random cropper";
        public string Category => "Internal";

        public const int TileSize = 256;

        public const string Filename = "images/equus_quagga.jpg";

        public static readonly Random Rnd = new Random();

        public Image RandomCrop(Image image, int tileSize)
        {
            var x = Rnd.Next(0, image.Width);
            var y = Rnd.Next(0, image.Height);

            var width = Math.Min(tileSize, image.Width - x);
            var height = Math.Min(tileSize, image.Height - y);

            return image.Crop(x, y, width, height);
        }

        public string Execute(string[] args)
        {
            NetVips.LeakSet(true);

            Parallel.For(0, 1000, new ParallelOptions {MaxDegreeOfParallelism = NetVips.ConcurrencyGet()},
                i =>
                {
                    using var input = File.OpenRead(Filename);

                    using var source = new SourceCustom();
                    source.OnRead += (buffer, length) => input.Read(buffer, 0, length);
                    source.OnSeek += (offset, origin) => input.Seek(offset, origin);

                    using var output = File.OpenWrite($"x_{i}.png");

                    using var target = new TargetCustom();
                    target.OnWrite += (buffer, length) =>
                    {
                        output.Write(buffer, 0, length);
                        return length;
                    };
                    target.OnFinish += () => output.Close();

                    using var image = Image.NewFromSource(source, access: Enums.Access.Sequential);
                    RandomCrop(image, TileSize).WriteToTarget(target, ".png");
                }
            );

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return "Done!";
        }
    }
}