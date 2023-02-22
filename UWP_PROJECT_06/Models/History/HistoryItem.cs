using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.History
{
    public class HistoryItem
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string FullPath { get; set; }
        public DateTime Date { get; set; }

    }
}
