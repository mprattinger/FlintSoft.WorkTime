using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintSoft.WorkTime.Models
{
    public class WorkTimeInfo
    {
        /// <summary>
        /// Date for this info
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// Target work time for this day
        /// </summary>
        public TimeSpan TargetWorkTime { get; set; }

        /// <summary>
        /// Target pause for this day
        /// </summary>
        public TimeSpan TargetPauseTime { get; set; }

        /// <summary>
        /// Currently worked for this day
        /// </summary>
        public TimeSpan WorkedTime { get; set; }

        /// <summary>
        /// Current taken pause for this day
        /// </summary>
        public TimeSpan PausedTime { get; set; }

        /// <summary>
        /// Remaining time to work
        /// </summary>
        public TimeSpan WorkTimeMissing { get; set; }

        /// <summary>
        /// Remaining pause
        /// </summary>
        public TimeSpan PausTimeMissing { get; set; }

        /// <summary>
        /// Is the person active aka working?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Estimated time to go home. Based on the data
        /// </summary>
        public DateTime Time2GoHome { get; set; }
    }
}
