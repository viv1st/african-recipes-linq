using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using African_recipes.collections;
using Newtonsoft.Json;

class Program
{
    static void Main()
    {
        // Lire le fichier JSON
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        string jsonData = File.ReadAllText(jsonFilePath);

        // Désérialiser le JSON en objets C#
        ListeRecettesData recettesData = JsonConvert.DeserializeObject<ListeRecettesData>(jsonData);

        // Créer le document XML
        XDocument xmlDocument = CreateXmlDocument(recettesData);

        // Afficher le document XML
        Console.WriteLine(xmlDocument);

        // Définir le chemin du fichier XML
        string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        string xmlDirectory = Path.Combine(projectRoot, "xml");
        string xmlFilePath = Path.Combine(xmlDirectory, "Recettes.xml");

        // Sauvegarder le document XML
        SaveXmlDocument(xmlDocument, xmlDirectory, xmlFilePath);

        // Lire le fichier XML et générer le fichier JSON
        ListeRecettesData recettesData2 = ConvertXmlToJson(xmlFilePath);

        // Sérialiser les objets C# en JSON
        string json = JsonConvert.SerializeObject(recettesData2, Formatting.Indented);

        // Afficher le JSON
        Console.WriteLine(json);

        // Définir le chemin du fichier JSON
        string jsonDirectory = Path.Combine(projectRoot, "json");
        string newjsonFilePath = Path.Combine(jsonDirectory, "Recettes.json");

        // Sauvegarder le document JSON
        SaveJsonDocument(json, jsonDirectory, newjsonFilePath);
    }

    static XDocument CreateXmlDocument(ListeRecettesData recettesData)
    {
        return new XDocument(
            new XElement("Recettes",
                from recette in recettesData.Recettes
                select new XElement("Recette",
                    new XElement("Nom", recette.Nom),
                    new XElement("Origine", recette.Origine),
                    new XElement("Ingrédients",
                        from ingredient in recette.Ingrédients
                        select new XElement("Ingrédient", ingredient)
                    ),
                    new XElement("Instructions",
                        from instruction in recette.Instructions
                        select new XElement("Étape", instruction)
                    )
                )
            )
        );
    }

    static void SaveXmlDocument(XDocument xmlDocument, string xmlDirectory, string xmlFilePath)
    {
        if (!Directory.Exists(xmlDirectory))
        {
            Directory.CreateDirectory(xmlDirectory);
        }
        xmlDocument.Save(xmlFilePath);
    }

    static ListeRecettesData ConvertXmlToJson(string xmlFilePath)
    {
        XDocument xmlDocument = XDocument.Load(xmlFilePath);
        return new ListeRecettesData
        {
            Recettes = (from recette in xmlDocument.Descendants("Recette")
                        select new Recette
                        {
                            Nom = recette.Element("Nom")?.Value,
                            Origine = recette.Element("Origine")?.Value,
                            Ingrédients = (from ingredient in recette.Element("Ingrédients")?.Elements("Ingrédient")
                                           select ingredient.Value).ToList(),
                            Instructions = (from instruction in recette.Element("Instructions")?.Elements("Étape")
                                            select instruction.Value).ToList()
                        }).ToList()
        };
    }

    static void SaveJsonDocument(string json, string jsonDirectory, string jsonFilePath)
    {
        if (!Directory.Exists(jsonDirectory))
        {
            Directory.CreateDirectory(jsonDirectory);
        }
        File.WriteAllText(jsonFilePath, json);
    }
}
