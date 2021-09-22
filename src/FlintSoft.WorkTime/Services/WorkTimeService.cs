using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintSoft.WorkTime.Services
{
    public interface IWorkTimeService
    {
        WorkTimeDayConfig GetWorkTimeForDay(DateTime date);
        TimeSpan GetTargetPauseForTimeSpan(TimeSpan workTime, bool isFriday = false);
    }

    public class WorkTimeService : IWorkTimeService
    {
        private readonly ILogger<WorkTimeService> _logger;
        private readonly IFeiertagService _feiertagService;
        private readonly WorkTimeConfig _config;

        public WorkTimeService(ILogger<WorkTimeService> logger,
            IFeiertagService feiertagService,
            WorkTimeConfig config)
        {
            _logger = logger;
            _feiertagService = feiertagService;
            _config = config;
        }

        public WorkTimeDayConfig GetWorkTimeForDay(DateTime date)
        {
            var cfg = _config.WorkDays.First(x => x.WorkDay == date.DayOfWeek);

            if (_feiertagService.IsFeiertag(date) || _feiertagService.IsFenstertag(date))
                cfg.TargetWorkTime = TimeSpan.Zero;

            return cfg;
        }

        public TimeSpan GetTargetPauseForTimeSpan(TimeSpan workTime, bool isFriday = false)
        {
            if (workTime.TotalHours >= 6)
            {
                if (isFriday)
                {
                    if (workTime <= TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(15))) {
                        return TimeSpan.FromMinutes(15);
                    }

                    if (workTime > TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(15)) && workTime < TimeSpan.FromHours(6).Add(TimeSpan.FromMinutes(30)))
                    {
                        return workTime.Subtract(TimeSpan.FromHours(6));
                    }

                    return TimeSpan.FromMinutes(30);
                }

                return TimeSpan.FromMinutes(30);
            }
            else
            {
                return TimeSpan.Zero;
            }
        }
    }
}
