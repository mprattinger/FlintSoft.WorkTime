using System;

namespace FlintSoft.WorkTime.Models
{
    public class WorkTimeDayConfig
    {
        public DayOfWeek WorkDay { get; set; }

        public TimeSpan TargetWorkTime { get; set; }
    }
}
