using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Notes
{
    public partial class Quote
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey("Source")]
        public int SourceID { get; set; }

        [Required]
        [StringLength(300)]
        public string QuoteBegin { get; set; }

        [Required]
        [StringLength(300)]
        public string QuoteEnd { get; set; }

        [Required]
        [StringLength(10000)]
        public string OriginalQuote { get; set; }

        [Required]
        [StringLength(10000)]
        public string TranslatedQuote { get; set; }

    }
}
