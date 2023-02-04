using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary
{
    internal class Language
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("Language")]
        [Required]
        [StringLength(100)]
        public string Language1 { get; set; }
    }
}
