using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Bookmarks
{
    public partial class DailyTask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey("Bookmark")]
        public int BookmarkID { get; set; }

        public DateTime TimeBegin { get; set; }
        public DateTime TimeEnd { get; set; }

        [Required]
        [StringLength(1000)]
        public string Task { get; set; }
    }
}
