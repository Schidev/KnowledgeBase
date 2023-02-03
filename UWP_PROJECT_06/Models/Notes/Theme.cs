using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Notes
{
    public partial class Theme
    {
        [PrimaryKey, AutoIncrement]
        public byte Id { get; set; }

        [Column("Theme")]
        [Required]
        [StringLength(20)]
        public string Theme1 { get; set; }

    }
}
