using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using African_recipes.collections;

namespace AfricanRecipes
{
    public class Functions
    {
        public static void JsonFileToXmlFile(string jsonFilePath, string xmlFilePath)
        {
            // Lire le contenu du fichier JSON
            var json = File.ReadAllText(jsonFilePath);

            // Parser le JSON en un objet JObject
            var jsonObject = JObject.Parse(json);

            // Utiliser LINQ to XML pour construire l'élément XML
            var xml = new XElement("Recettes",
                from item in jsonObject["Recettes"]
                select new XElement("Recette",
                    new XElement("Nom", (string)item["Nom"]),
                    new XElement("Origine", (string)item["Origine"]),
                    new XElement("Ingrédients",
                        from ingredient in item["Ingrédients"]
                        select new XElement("Ingrédient", (string)ingredient)
                    ),
                    new XElement("Instructions",
                        from instruction in item["Instructions"]
                        select new XElement("Étape", (string)instruction)
                    )
                )
            );

            // Sauvegarder le contenu XML dans un fichier
            xml.Save(xmlFilePath);
        }

        public static void XmlFileToJsonFile(string xmlFilePath, string jsonFilePath)
        {
            // Lire le contenu du fichier XML
            var xml = XDocument.Load(xmlFilePath);

            // Utiliser LINQ to XML pour transformer l'XML en un JArray
            var jsonArray = new JArray(
                from recette in xml.Descendants("Recette")
                select new JObject(
                    new JProperty("Nom", recette.Element("Nom")?.Value),
                    new JProperty("Origine", recette.Element("Origine")?.Value),
                    new JProperty("Ingrédients",
                        new JArray(
                            from ingredient in recette.Element("Ingrédients")?.Elements("Ingrédient")
                            select ingredient.Value
                        )
                    ),
                    new JProperty("Instructions",
                        new JArray(
                            from instruction in recette.Element("Instructions")?.Elements("Étape")
                            select instruction.Value
                        )
                    )
                )
            );

            // Convertir l'objet JArray en chaîne JSON
            var json = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

            // Sauvegarder le contenu JSON dans un fichier
            File.WriteAllText(jsonFilePath, json);
        }

        public static ListeRecettesData ReadDataFromJson(string jsonFilePath)
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<ListeRecettesData>(jsonData);
        }

        public static void DisplayRecettes(IEnumerable<Recette> recettes)
        {
            foreach (var recette in recettes)
            {
                Console.WriteLine($"Nom: {recette.Nom}, Origine: {recette.Origine}");
                Console.WriteLine("Ingrédients: " + string.Join(", ", recette.Ingrédients));
                Console.WriteLine("Instructions: " + string.Join(" | ", recette.Instructions));
                Console.WriteLine();
            }
        }

        public static IEnumerable<Recette> SearchRecettes(ListeRecettesData recettesData, string searchTerm)
        {
            return recettesData.Recettes.Where(r =>
                r.Nom.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                r.Origine.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                r.Ingrédients.Any(i => i.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                r.Instructions.Any(i => i.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            );
        }

        public static IEnumerable<Recette> SortRecettes(IEnumerable<Recette> recettes, string sortBy)
        {
            return sortBy switch
            {
                "Nom" => recettes.OrderBy(r => r.Nom),
                "Origine" => recettes.OrderBy(r => r.Origine),
                _ => recettes
            };
        }

        public static IEnumerable<Recette> FilterRecettes(IEnumerable<Recette> recettes, string field, string comparisonOperator, string value)
        {
            return recettes.Where(r =>
            {
                string fieldValue = field switch
                {
                    "Nom" => r.Nom,
                    "Origine" => r.Origine,
                    "Ingrédients" => string.Join(", ", r.Ingrédients),
                    "Instructions" => string.Join(" | ", r.Instructions),
                    _ => null
                };

                return fieldValue != null && Compare(fieldValue, comparisonOperator, value);
            });
        }

        private static bool Compare(string fieldValue, string comparisonOperator, string value)
        {
            return comparisonOperator switch
            {
                "contient" => fieldValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0,
                "commence par" => fieldValue.StartsWith(value, StringComparison.OrdinalIgnoreCase),
                "se termine par" => fieldValue.EndsWith(value, StringComparison.OrdinalIgnoreCase),
                "égal" => fieldValue.Equals(value, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        public static XDocument CreateXmlDocument(ListeRecettesData recettesData)
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

        public static void SaveXmlDocument(XDocument xmlDocument, string xmlDirectory, string xmlFilePath)
        {
            if (!Directory.Exists(xmlDirectory))
            {
                Directory.CreateDirectory(xmlDirectory);
            }
            xmlDocument.Save(xmlFilePath);
        }

        public static ListeRecettesData ConvertXmlToJson(string xmlFilePath)
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

        public static void SaveJsonDocument(string json, string jsonDirectory, string jsonFilePath)
        {
            if (!Directory.Exists(jsonDirectory))
            {
                Directory.CreateDirectory(jsonDirectory);
            }
            File.WriteAllText(jsonFilePath, json);
        }
    }
}