using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Notes
{
    public partial class State
    {
        [PrimaryKey, AutoIncrement]
        public byte Id { get; set; }

        [Column("State")]
        [Required]
        [StringLength(15)]
        public string State1 { get; set; }

    }
}
