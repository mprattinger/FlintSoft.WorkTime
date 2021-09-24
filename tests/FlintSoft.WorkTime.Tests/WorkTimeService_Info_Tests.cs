using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FlintSoft.WorkTime.Tests
{
    public class WorkTimeService_Info_Tests
    {
        private readonly IWorkTimeService _workTimeService;

        public WorkTimeService_Info_Tests()
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
        
        [Fact]
        public void FullWorkDayNoOvertime()
        {
            //_workTimeService.GetWorkTimeInfo(new DateTime(2022, 09, 22), TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(12)), TimeSpan.FromMinutes(30));
        }
    }
}
