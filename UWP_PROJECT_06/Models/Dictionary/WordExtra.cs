using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary
{
    [Table("WordsExtras")]
    public partial class WordExtra
    {
        [Key]
        public int RowID { get; set; }

        public int WordId { get; set; }

        public int LinkedWordId { get; set; }

        public int LinkType { get; set; }

        [Required]
        [StringLength(3000)]
        public string ExtraText { get; set; }
    }
}
