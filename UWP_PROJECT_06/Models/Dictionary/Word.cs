using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Dictionary
{
    public partial class Word
    {
        public int Id { get; set; }

        [Column("Word")]
        [Required]
        [StringLength(255)]
        public string Word1 { get; set; }

        public int Language { get; set; }

        public int Status { get; set; }

        public int PartOfSpeech { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public DateTime LastRepeatedOn { get; set; }
    }
}
