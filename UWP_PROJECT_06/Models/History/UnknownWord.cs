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
    public partial class UnknownWord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Word { get; set; }

        [ForeignKey("Language")]
        public int Language { get; set; }

        public DateTime LastModifiedOn { get; set; }
    }
}
