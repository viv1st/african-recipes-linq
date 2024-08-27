using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AfricanRecipes
{
    public class Functions
    {
        public static string GetFileDirectory(string relativePath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }
        public static void JsonFileToXmlFile(string jsonFilePath, string xmlFilePath)
        {
            // Lire le contenu du fichier JSON
            var json = File.ReadAllText(jsonFilePath);

            // Parser le JSON en un objet JObject
            var jsonObject = JArray.Parse(json);

            // Utiliser LINQ to XML pour construire l'élément XML
            var xml = new XElement("Recettes",
                from item in jsonObject
                select new XElement("Recette",
                    from prop in ((JObject)item).Properties()
                    select new XElement(prop.Name, prop.Value)
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
                    recette.Elements().Select(e => new JProperty(e.Name.LocalName, e.Value))
                )
            );

            // Convertir l'objet JArray en chaîne JSON
            var json = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

            // Sauvegarder le contenu JSON dans un fichier
            File.WriteAllText(jsonFilePath, json);
        }


    }
}
