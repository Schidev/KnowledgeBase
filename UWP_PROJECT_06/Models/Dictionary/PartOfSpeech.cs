using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary
{
    public partial class PartOfSpeech
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("PartOfSpeech")]
        [Required]
        [StringLength(100)]
        public string PartOfSpeech1 { get; set; }
    }
}
