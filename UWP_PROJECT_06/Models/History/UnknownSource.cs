using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.History
{
    public partial class UnknownSource
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Source { get; set; }

        [ForeignKey("SourceType")]
        public int SourceType { get; set; }

        public DateTime LastModifiedOn { get; set; }
    }
}
