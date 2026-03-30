namespace sample1;
using Newtonsoft.Json;

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
    }
}
