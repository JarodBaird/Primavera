using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Primavera.Parsers.Util
{
    public static class FileHelper
    {
        public static void OutputToFile(string fileName, object output)
        {
            string json = JsonConvert.SerializeObject(output);
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
            Directory.CreateDirectory(outputDirectory);

            string path = Path.Combine(outputDirectory, $"{fileName}.json");
            FileStream stream = File.Create(path);

            byte[] encoded = Encoding.UTF8.GetBytes(json);

            stream.Write(encoded, 0, encoded.Length);
            stream.Dispose();
        }
    }
}