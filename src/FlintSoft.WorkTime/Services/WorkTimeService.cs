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
        (List<(DateTime start, DateTime end)> worked, List<(DateTime start, DateTime end)> paused) PrepareCheckins(List<CheckInItem> checkInItems);
        TimeSpan CalculateWorkTime(List<(DateTime start, DateTime end)> worked);
        TimeSpan CalculatePauseTime(List<(DateTime start, DateTime end)> paused);
        bool IsActive(List<CheckInItem> checkInItems);
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

        public WorkTimeInfo GetWorkTimeInfo(DateTime workDay, List<CheckInItem> checkInItems)
        {
            WorkTimeInfo ret = null;

            try
            {
                //Prepare checkins
                checkInItems.Sort((c1, c2) => DateTime.Compare(c1.CheckinTime, c2.CheckinTime));

                //Calculate Targets
                ret.TargetWorkTime = GetWorkTimeTargetForDay(workDay);
                ret.TargetPauseTime = GetTargetPauseForTimeSpan(ret.TargetWorkTime, workDay.DayOfWeek == DayOfWeek.Friday);

                //Calculate Actual
                var actualData = PrepareCheckins(checkInItems);
                ret.WorkedTime = CalculateWorkTime(actualData.worked);
                ret.PausedTime = CalculatePauseTime(actualData.paused);
                ret.IsActive = IsActive(checkInItems);

                ret.StartOfWork = checkInItems.First().CheckinTime;

                //Calculate Diffs
                
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
        public (List<(DateTime start, DateTime end)> worked, List<(DateTime start, DateTime end)> paused) PrepareCheckins(List<CheckInItem> checkInItems)
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
        public void CalculateTimeToGoHome()
        {

        }
        #endregion
    }
}
