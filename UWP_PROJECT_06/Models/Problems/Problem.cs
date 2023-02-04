using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_PROJECT_06.Models.Problems
{
    public partial class Problem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsDone { get; set; }

        [Column("Problem")]
        [Required]
        [StringLength(255)]
        public string Problem1 { get; set; }

        [Required]
        [StringLength(255)]
        public string Link { get; set; }

        public byte Week { get; set; }

        [Required]
        [StringLength(1)]
        public string Category { get; set; }

        [Required]
        [StringLength(1)]
        public string TimePeriodType { get; set; }
        public DateTime DueDateTimeBegin { get; set; }
        public DateTime DueDateTimeEnd { get; set; }

        public bool IsMonday { get; set; }
        public bool IsTuesday { get; set; }
        public bool IsWednesday { get; set; }
        public bool IsThursday { get; set; }
        public bool IsFriday { get; set; }
        public bool IsSaturday { get; set; }
        public bool IsSunday { get; set; }

        public byte RepetitionFrequencyWeeks { get; set; }
        public byte RepetitionFrequencyDays { get; set; }

        public DateTime RepetitionDateFrom { get; set; }
        public DateTime RepetitionDateTo { get; set; }

        [Required]
        [StringLength(100)]
        public string Hash { get; set; }



        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

    }

}