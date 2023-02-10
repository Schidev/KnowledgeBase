using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary.OnlineDictionary
{
    public class Definition
    {
        public string _word { get; set; }
        public string _gender { get; set; }
        public string _plural { get; set; }
        public int _part_of_speech { get; set; }
        public string _meaning_string { get; set; }

        public Meaning _definitions { get; set; }
        public List<Example> _examples { get; set; }
        public Verb _verb { get; set; }

        public Definition()
        {
            _word = string.Empty;
            _gender = string.Empty;
            _plural = string.Empty;
            _part_of_speech = 0;
            _meaning_string = string.Empty;

            _definitions = new Meaning();
            _examples = new List<Example>();
            _verb = new Verb();
        }
    }
}
