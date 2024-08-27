using African_recipes.collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AfricanRecipes
{
    class Program
    {
        static void Main()
        {
            // Lire les données JSON initiales
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            var recettesData = Functions.ReadDataFromJson(jsonFilePath);

            // Copier les données pour les utiliser dans différentes étapes
            var searchData = new ListeRecettesData
            {
                Recettes = new List<Recette>(recettesData.Recettes)
            };

            var sortData = new ListeRecettesData
            {
                Recettes = new List<Recette>(recettesData.Recettes)
            };

            var filterData = new ListeRecettesData
            {
                Recettes = new List<Recette>(recettesData.Recettes)
            };

            // 1. Recherche globale
            Console.Write("Entrez un mot-clé pour rechercher une recette : ");
            string searchTerm = Console.ReadLine();
            var searchResults = Functions.SearchRecettes(searchData, searchTerm);

            if (!searchResults.Any())
            {
                Console.WriteLine($"\nAucune recette trouvée pour le mot-clé '{searchTerm}'.");
            }
            else
            {
                Console.WriteLine($"\nRésultats de la recherche pour '{searchTerm}':");
                Functions.DisplayRecettes(searchResults);

                // 2. Tri des résultats de recherche
                Console.Write("\nEntrez le champ de tri (Nom, Origine) : ");
                string sortBy = Console.ReadLine();
                var sortedResults = Functions.SortRecettes(searchResults, sortBy);

                Console.WriteLine($"\nRecettes triées par {sortBy}:");
                Functions.DisplayRecettes(sortedResults);

                // 3. Filtrage des résultats triés
                Console.Write("\nEntrez le champ à filtrer (Nom, Origine, Ingrédients, Instructions) : ");
                string filterField = Console.ReadLine();

                Console.Write("\nEntrez l'opérateur de comparaison (contient, commence par, se termine par, égal) : ");
                string comparisonOperator = Console.ReadLine();

                Console.Write("\nEntrez la valeur de comparaison : ");
                string value = Console.ReadLine();

                var filteredResults = Functions.FilterRecettes(sortedResults, filterField, comparisonOperator, value);

                Console.WriteLine($"\nRecettes filtrées par {filterField} avec la condition '{comparisonOperator} {value}':");
                Functions.DisplayRecettes(filteredResults);
            }
        }
    }
}