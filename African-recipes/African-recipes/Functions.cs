using African_recipes.collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FunctionsNamespace
{
    class Functions
    {
        public static string ProjectRoot()
        {
            return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
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

        private static string GetFilePath(string filename, string directory = "")
        {
            string filePath = "";
            if (directory == "")
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            }
            else
            {
                string jsonDirectory = Path.Combine(ProjectRoot(), directory);
                filePath = Path.Combine(jsonDirectory, filename);
            }

            return filePath;
        }

        public static ListeRecettesData ReadJsonData(string filename, string directory = "")
        {
            string jsonFilePath = GetFilePath(filename, directory);            
            string jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<ListeRecettesData>(jsonData);
        }

        public static ListeRecettesData ReadXmlData(string filename, string directory = "")
        {
            string xmlFilePath = GetFilePath(filename, directory);
            string jsonData = File.ReadAllText(xmlFilePath);
            return JsonConvert.DeserializeObject<ListeRecettesData>(jsonData);
        }


        public static ListeRecettesData ReadData()
        {
            // Lire le fichier JSON
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            string jsonData = File.ReadAllText(jsonFilePath);
            var recettesData = JsonConvert.DeserializeObject<ListeRecettesData>(jsonData);

            // Lire le fichier XML
            string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recettes.xml");
            if (File.Exists(xmlFilePath))
            {
                XDocument xmlDocument = XDocument.Load(xmlFilePath);
                var xmlRecettes = (from recette in xmlDocument.Descendants("Recette")
                                   select new Recette
                                   {
                                       Nom = recette.Element("Nom")?.Value,
                                       Origine = recette.Element("Origine")?.Value,
                                       Ingrédients = (from ingredient in recette.Element("Ingrédients")?.Elements("Ingrédient")
                                                      select ingredient.Value).ToList(),
                                       Instructions = (from instruction in recette.Element("Instructions")?.Elements("Étape")
                                                       select instruction.Value).ToList()
                                   }).ToList();
                recettesData.Recettes.AddRange(xmlRecettes);
            }

            return recettesData;
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

        public static IEnumerable<Recette> FilterRecettes(IEnumerable<Recette> recettes, Func<Recette, bool> condition)
        {
            return recettes.Where(condition);
        }
    }
}
