using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using African_recipes.collections;
using Newtonsoft.Json;
using FunctionsNamespace;

class Program
{
    static void Main()
    {
        // ---- DEBUT Réupération des données sous format XML et JSON ----
        

        // Récup données à partir du JSON


        // ---- FIN Réupération des données sous format XML et JSON ----

        // Lire les données JSON et XML
        var recettesData = Functions.ReadData();

        // Exemple de recherche
        var searchResults = Functions.SearchRecettes(recettesData, "Poulet");

        // Exemple de tri
        var sortedResults = Functions.SortRecettes(searchResults, "Nom");

        // Exemple de condition de recherche
        var filteredResults = Functions.FilterRecettes(sortedResults, r => r.Nom.Contains("Poulet"));

        // Afficher les résultats
        foreach (var recette in filteredResults)
        {
            Console.WriteLine($"Nom: {recette.Nom}, Origine: {recette.Origine}");
        }

        // Lire le fichier JSON
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        string jsonData = File.ReadAllText(jsonFilePath);

        // Désérialiser le JSON en objets C#
        ListeRecettesData recettesDataInitial = JsonConvert.DeserializeObject<ListeRecettesData>(jsonData);

        // Créer le document XML
        XDocument xmlDocument = Functions.CreateXmlDocument(recettesDataInitial);

        // Afficher le document XML
        Console.WriteLine(xmlDocument);

        // Définir le chemin du fichier XML
        string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        string xmlDirectory = Path.Combine(projectRoot, "xml");
        string xmlFilePath = Path.Combine(xmlDirectory, "Recettes.xml");

        // Sauvegarder le document XML
        Functions.SaveXmlDocument(xmlDocument, xmlDirectory, xmlFilePath);

        // Lire le fichier XML et générer le fichier JSON
        ListeRecettesData recettesData2 = Functions.ConvertXmlToJson(xmlFilePath);

        // Sérialiser les objets C# en JSON
        string json = JsonConvert.SerializeObject(recettesData2, Formatting.Indented);

        // Afficher le JSON
        Console.WriteLine(json);

        // Définir le chemin du fichier JSON
        string jsonDirectory = Path.Combine(projectRoot, "json");
        string newjsonFilePath = Path.Combine(jsonDirectory, "Recettes.json");

        // Sauvegarder le document JSON
        Functions.SaveJsonDocument(json, jsonDirectory, newjsonFilePath);
    }

}
