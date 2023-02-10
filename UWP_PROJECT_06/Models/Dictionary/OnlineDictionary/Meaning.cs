using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary.OnlineDictionary
{
    public class Meaning
    {
        public string _theme { get; set; }
        public string _meaning { get; set; }
        public List<string> _translation { get; set; }

        public Meaning()
        {
            _theme = String.Empty;
            _meaning = String.Empty;
            _translation = new List<String>();
        }
    }
}
