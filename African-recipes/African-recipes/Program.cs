using AfricanRecipes;

class Program
{
    static void Main(string[] args)
    {
        string jsonFilePath = Functions.GetFileDirectory("/data_source.json");
        string xmlFilePath = Functions.GetFileDirectory("/data_source.xml");

        string newJsonFilePath = "transformed_data_source.json";
        string newXmlFilePath = "transformed_data_source.xml";

        // Convertir JSON en XML
        Functions.JsonFileToXmlFile(jsonFilePath, newXmlFilePath);
        Console.WriteLine("Le fichier JSON a été converti en XML.");

        // Convertir XML en JSON
        Functions.XmlFileToJsonFile(xmlFilePath, newJsonFilePath);
        Console.WriteLine("Le fichier XML a été converti en JSON.");
    }
}