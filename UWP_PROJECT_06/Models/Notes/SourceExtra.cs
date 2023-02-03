using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Notes
{
    public partial class SourceExtra
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey("Source")]
        public int SourceID { get; set; }

        [Required]
        [StringLength(5000)]
        public string Key { get; set; }

        [Required]
        [StringLength(5000)]
        public string Value { get; set; }
    }
}
