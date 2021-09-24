using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintSoft.WorkTime.Models
{
    public class CheckInItem
    {
        /// <summary>
        /// CheckinDate
        /// </summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// Zeit des Checkin
        /// </summary>
        public DateTime CheckinTime { get; set; }
    }
}
