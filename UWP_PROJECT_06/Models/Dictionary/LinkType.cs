using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary
{
    public partial class LinkType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("LinkType")]
        [Required]
        [StringLength(100)]
        public string LinkType1 { get; set; }
    }
}
