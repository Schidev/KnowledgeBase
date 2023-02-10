using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary.OnlineDictionary
{
    public class Verb
    {
        public string _prasens { get; set; }
        public string _prateritum { get; set; }
        public string _perfekt { get; set; }

        public Verb()
        {
            _prasens = string.Empty;
            _prateritum = string.Empty;
            _perfekt = string.Empty;
        }
    }
}
