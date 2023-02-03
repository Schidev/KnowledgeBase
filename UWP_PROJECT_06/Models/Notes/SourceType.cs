using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Notes
{
    public partial class SourceType
    {
        [PrimaryKey, AutoIncrement]
        public byte Id { get; set; }

        [Column("SourceType")]
        [Required]
        [StringLength(15)]
        public string SourceType1 { get; set; }

    }
}
