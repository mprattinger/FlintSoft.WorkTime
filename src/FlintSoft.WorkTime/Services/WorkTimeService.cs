using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Extensions;
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
        TimeSpan GetWorkTimeTargetForDay(DateTime date);
        TimeSpan GetTargetPauseForTimeSpan(TimeSpan workTime, bool isFriday = false);
        WorkTimeInfo GetWorkTimeInfo(DateTime workDay, List<CheckInItem> checkInItems);
        (List<(DateTime start, DateTime end)> worked, List<(DateTime start, DateTime end)> paused) PrepareCheckins(List<CheckInItem> checkInItems, bool useNow = false);
        TimeSpan CalculateWorkTime(List<(DateTime start, DateTime end)> worked);
        TimeSpan CalculatePauseTime(List<(DateTime start, DateTime end)> paused);
        bool IsActive(List<CheckInItem> checkInItems);
        DateTime CalculateTimeToGoHome(WorkTimeInfo info);
    }

    public class WorkTimeService : IWorkTimeService
    {
        private readonly ILogger<WorkTimeService> _logger;
        private readonly IFeiertagService _feiertagService;
        private readonly WorkTimeConfig _config;
        private readonly ISystemTime _systemTime;

        public WorkTimeService(ILogger<WorkTimeService> logger,
            IFeiertagService feiertagService,
            WorkTimeConfig config,
            ISystemTime systemTime)
        {
            _logger = logger;
            _feiertagService = feiertagService;
            _config = config;
            _systemTime = systemTime;
        }

        public WorkTimeInfo GetWorkTimeInfo(DateTime workDay, List<CheckInItem> checkInItems)
        {
            WorkTimeInfo ret = new();

            try
            {
                //Prepare checkins
                checkInItems.Sort((c1, c2) => DateTime.Compare(c1.CheckinTime, c2.CheckinTime));

                ret.Day = workDay;

                //Calculate Targets
                ret.TargetWorkTime = GetWorkTimeTargetForDay(workDay);
                ret.TargetPauseTime = GetTargetPauseForTimeSpan(ret.TargetWorkTime, workDay.DayOfWeek == DayOfWeek.Friday);

                //Calculate Actual
                var (worked, paused) = PrepareCheckins(checkInItems);

                ret.WorkedTime = CalculateWorkTime(worked);
                
                ret.PausedTime = CalculatePauseTime(paused);
                if (ret.PausedTime >= TimeSpan.FromMinutes(10))
                {
                    //Ab 10 Minuten Pause werden von der Firma 10 Minuten geschenkt
                    ret.TargetPauseTime = ret.TargetPauseTime.Subtract(TimeSpan.FromMinutes(10));
                }

                ret.IsActive = IsActive(checkInItems);

                ret.StartOfWork = checkInItems.First().CheckinTime;

                ret.Time2GoHome = CalculateTimeToGoHome(ret);
                
            }
            catch (Exception)
            {

                throw;
            }

            return ret;
        }

        #region Target
        public TimeSpan GetWorkTimeTargetForDay(DateTime date)
        {
            var ret = TimeSpan.Zero;

            _logger.LogDebug($"Loading target data for workday {date.DayOfWeek}...");
            ret = _config.WorkDays.First(x => x.WorkDay == date.DayOfWeek).TargetWorkTime;

            _logger.LogDebug($"Checking if date is holiday or bridging day...");
            if (_feiertagService.IsFeiertag(date) || _feiertagService.IsFenstertag(date))
                ret = TimeSpan.Zero;

            return ret;
        }

        public TimeSpan GetTargetPauseForTimeSpan(TimeSpan workTime, bool isFriday = false)
        {
            _logger.LogDebug($"Calculating target pause for the given worktime...");
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
        #endregion

        #region Actual
        public (List<(DateTime start, DateTime end)> worked, List<(DateTime start, DateTime end)> paused) PrepareCheckins(List<CheckInItem> checkInItems, bool useNow = false)
        {
            var worked = new List<(DateTime start, DateTime end)>();
            var paused = new List<(DateTime start, DateTime end)>();

            _logger.LogDebug($"Calculating workime for {checkInItems.Count} checkins...");

            (DateTime start, DateTime end) currWorked = new();
            (DateTime start, DateTime end) currPaused = new();

            for (int i = 0; i < checkInItems.Count; i++)
            {
                if (i == 0)
                {
                    currWorked.start = checkInItems[i].CheckinTime;
                }
                else
                {
                    if (i % 2 == 1)
                    {
                        currWorked.end = checkInItems[i].CheckinTime;
                        worked.Add(currWorked);
                        currWorked = new();
                        currPaused.start = checkInItems[i].CheckinTime;
                    }
                    else
                    {
                        currWorked.start = checkInItems[i].CheckinTime;
                        currPaused.end = checkInItems[i].CheckinTime;
                        paused.Add(currPaused);
                        currPaused = new();
                    }
                }
            }

            if(useNow)
            {
                if(currWorked.start != DateTime.MinValue && currWorked.end == DateTime.MinValue)
                {
                    //Offenes Ende -> aktuelle Zeit nehmen
                    currWorked.end = _systemTime.Now;
                    worked.Add(currWorked);
                }
            }

            return (worked, paused);
        }

        public TimeSpan CalculateWorkTime(List<(DateTime start, DateTime end)> worked)
        {
            var ret = TimeSpan.Zero;

            ret = new TimeSpan(worked.Select(x => x.end.Subtract(x.start)).Sum(x => x.Ticks));

            return ret;
        }

        public TimeSpan CalculatePauseTime(List<(DateTime start, DateTime end)> paused)
        {
            var ret = TimeSpan.Zero;

            ret = new TimeSpan(paused.Select(x => x.end.Subtract(x.start)).Sum(x => x.Ticks));

            return ret;
        }

        public bool IsActive(List<CheckInItem> checkInItems) => checkInItems.Count % 2 == 1;
        #endregion

        #region ETG
        public DateTime CalculateTimeToGoHome(WorkTimeInfo info)
        {
            var ttgh = info.StartOfWork;
            if (ttgh == DateTime.MinValue) return DateTime.MinValue;

            ttgh = ttgh.Add(info.WorkedTime).Add(info.WorkTimeMissing < TimeSpan.Zero ? TimeSpan.Zero : info.WorkTimeMissing);
            ttgh = ttgh.Add(info.PausedTime).Add(info.PauseTimeMissing < TimeSpan.FromMinutes(-10) ? TimeSpan.FromMinutes(-10) : info.PauseTimeMissing);
            
            return ttgh;
        }
        #endregion
    }
}
