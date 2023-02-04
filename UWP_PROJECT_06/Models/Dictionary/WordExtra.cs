using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace UWP_PROJECT_06.Models.Dictionary
{
    [SQLite.Table("WordsExtras")]
    public partial class WordExtra
    {
        [PrimaryKey, AutoIncrement]
        public int RowID { get; set; }

        [ForeignKey("Word")]
        public int WordId { get; set; }

        [ForeignKey("Word")]
        public int LinkedWordId { get; set; }

        [ForeignKey("LinkType")]
        public int LinkType { get; set; }

        [Required]
        [StringLength(3000)]
        public string ExtraText { get; set; }
    }
}
