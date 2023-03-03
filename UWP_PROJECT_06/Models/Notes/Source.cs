using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UWP_PROJECT_06.Models.Notes
{
    public partial class Source
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string SourceName { get; set; }

        public int Duration { get; set; }
        
        public int ActualTime { get; set; }

        [ForeignKey("State")]
        public byte State { get; set; }
        
        [ForeignKey("Theme")]
        public byte Theme { get; set; }
        
        [ForeignKey("SourceType")]
        public byte SourceType { get; set; }
        
        public bool IsDownloaded { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(1000)]
        public string SourceLink { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

    }
}
