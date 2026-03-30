namespace sample1;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
public class Personne
{
    public required string nom { get; set; }
    public required int age { get; set; }

    public string Hello(bool isLowercase)
    {
        string message = $"hello {nom}, you are {age}";

        if (isLowercase)
            return message;
        else
            return message.ToUpper();
    }
}
class Program
{
    static void Main(string[] args)
    {
        Personne p = new Personne
        {
            nom = "Alice",
            age = 25
        };

        string json = JsonConvert.SerializeObject(p, Formatting.Indented);

        Console.WriteLine(json);

        string inputFolder = @"imgs";
        string outputFolder = @"resized-imgs";
        string[] files = Directory.GetFiles(inputFolder);

        Stopwatch swSeq = Stopwatch.StartNew();
        foreach (string file in files)
        {
            using Image image = Image.Load(file);
            image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
            image.Save(Path.Combine(outputFolder, "resized_seq" + Path.GetFileName(file)));
        }
        swSeq.Stop();
        Console.WriteLine($"Processing the image seq {swSeq.ElapsedMilliseconds} ms");

        Stopwatch swPara = Stopwatch.StartNew();
        Parallel.ForEach(files, file =>
        {
            using Image image = Image.Load(file);
            image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
            image.Save(Path.Combine(outputFolder, "resized_par" + Path.GetFileName(file)));
        });
        swPara.Stop();
        Console.WriteLine($"Processing time parallel: {swPara.ElapsedMilliseconds} ms");
    }
}
