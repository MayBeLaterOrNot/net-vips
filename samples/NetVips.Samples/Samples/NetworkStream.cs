namespace NetVips.Samples
{
    using System;
    using System.IO;
    using System.Net;

    public class NetworkStream : ISample
    {
        public string Name => "Network stream";
        public string Category => "Streaming";

        public const string Uri = "https://images.weserv.nl/zebra.jpg";

        public void Execute(string[] args)
        {
            using var web = new WebClient();
            using var stream = web.OpenRead(Uri);
            var image = Image.NewFromStream(stream, access: Enums.Access.Sequential);
            Console.WriteLine(image.ToString());

            using var output = File.OpenWrite("stream-network.jpg");
            image.WriteToStream(output, ".jpg");

            Console.WriteLine("See stream-network.jpg");
        }
    }
}