using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace African_recipes.collections
{
    public class Recette
    {
        public string Nom { get; set; }
        public string Origine { get; set; }
        public List<string> Ingrédients { get; set; }
        public List<string> Instructions { get; set; }
    }
}
