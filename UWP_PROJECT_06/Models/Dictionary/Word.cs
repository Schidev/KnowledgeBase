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
    public partial class Word
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.Column("Word")]
        [Required]
        [StringLength(255)]
        public string Word1 { get; set; }

        [ForeignKey("Language")]
        public int Language { get; set; }

        [ForeignKey("Status")]
        public int Status { get; set; }

        [ForeignKey("PartOfSpeech")]
        public int PartOfSpeech { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public DateTime LastRepeatedOn { get; set; }
    }
}
