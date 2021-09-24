using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintSoft.WorkTime.Tests.Actual
{
    public class WorkTimeService_ActualPauseTime_Tests
    {
        private readonly WorkTimeService _workTimeService;

        public WorkTimeService_ActualPauseTime_Tests()
        {
            var cfg = new WorkTimeConfig
            {
                WorkDays = new List<WorkTimeDayConfig>() {
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Monday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Tuesday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Wednesday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Thursday, TargetWorkTime = TimeSpan.FromHours(8.2) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Friday, TargetWorkTime = TimeSpan.FromHours(5.7) },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Saturday, TargetWorkTime = TimeSpan.Zero },
                    new WorkTimeDayConfig() { WorkDay = DayOfWeek.Sunday, TargetWorkTime = TimeSpan.Zero }
                }
            };

            _workTimeService = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), cfg);
        }
    }
}
