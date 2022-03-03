using FlintSoft.Tools.Feiertage;
using FlintSoft.WorkTime.Models;
using FlintSoft.WorkTime.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlintSoft.WorkTime.Tests.Actual
{
    public class WorkTimeService_PrepareCheckins_Tests
    {
        private WorkTimeConfig _cfg;

        public WorkTimeService_PrepareCheckins_Tests()
        {
            _cfg = new WorkTimeConfig
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
        
        }

        [Fact]
        public void NoTimeNoWorkPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 8, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(0);
            paused.Count.Should().Be(0);
        }

        [Fact]
        public void NoTimeNoWorkPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 8, 0, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>();

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(0);
            paused.Count.Should().Be(0);
        }

        [Fact]
        public void OneTimeNoWorkPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 7, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(0);
            paused.Count.Should().Be(0);
        }


        [Fact]
        public void OneTimeNoWorkPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 7, 11, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(1);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 7, 11, 0));
            paused.Count.Should().Be(0);
        }

        [Fact]
        public void TwoTimesWorkTimeNoPauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 20, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(1);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.Count.Should().Be(0);
        }

        [Fact]
        public void TwoTimesWorkTimeNoPauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 20, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 9, 22, 11, 20, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(1);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.Count.Should().Be(0);
        }

        [Fact]
        public void ThreeTimesWorkTimePauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 43, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 43, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(1);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.Count.Should().Be(1);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
        }

        [Fact]
        public void ThreeTimesWorkTimePauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 12, 43, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 43, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(2);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            worked.Last().start.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            worked.Last().end.Should().Be(new DateTime(2021, 9, 22, 12, 43, 0));
            paused.Count.Should().Be(1);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
        }

        [Fact]
        public void FourTimesWorkTimePauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 43, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 48, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(2);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            worked.Last().start.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            worked.Last().end.Should().Be(new DateTime(2021, 9, 22, 14, 48, 0));
            paused.Count.Should().Be(1);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
        }

        [Fact]
        public void FourTimesWorkTimePauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 43, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 48, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(2);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            worked.Last().start.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            worked.Last().end.Should().Be(new DateTime(2021, 9, 22, 14, 48, 0));
            paused.Count.Should().Be(1);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
        }


        [Fact]
        public void FiveTimesWorkTimePauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 43, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 48, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 58, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(2);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            worked.Last().start.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            worked.Last().end.Should().Be(new DateTime(2021, 9, 22, 14, 48, 0));
            paused.Count.Should().Be(2);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            paused.Last().start.Should().Be(new DateTime(2021, 9, 22, 14, 48, 0));
            paused.Last().end.Should().Be(new DateTime(2021, 9, 22, 14, 58, 0));
        }

        [Fact]
        public void FiveTimesWorkTimePauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 15, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 11, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 20, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 43, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 48, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 58, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(3);
            worked.First().start.Should().Be(new DateTime(2021, 9, 22, 6, 11, 0));
            worked.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            worked[1].start.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            worked[1].end.Should().Be(new DateTime(2021, 9, 22, 14, 48, 0));
            worked[2].start.Should().Be(new DateTime(2021, 9, 22, 14, 58, 0));
            worked[2].end.Should().Be(new DateTime(2021, 9, 22, 15, 0, 0));
            paused.Count.Should().Be(2);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 20, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 43, 0));
            paused.Last().start.Should().Be(new DateTime(2021, 9, 22, 14, 48, 0));
            paused.Last().end.Should().Be(new DateTime(2021, 9, 22, 14, 58, 0));
        }

        [Fact]
        public void SixTimesWorkTimePauseTime()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data);

            worked.Count.Should().Be(3);
            worked[0].start.Should().Be(new DateTime(2021, 9, 22, 6, 33, 0));
            worked[0].end.Should().Be(new DateTime(2021, 9, 22, 11, 19, 0));
            worked[1].start.Should().Be(new DateTime(2021, 9, 22, 11, 36, 0));
            worked[1].end.Should().Be(new DateTime(2021, 9, 22, 14, 47, 0));
            worked[2].start.Should().Be(new DateTime(2021, 9, 22, 15, 01, 0));
            worked[2].end.Should().Be(new DateTime(2021, 9, 22, 15, 37, 0));
            paused.Count.Should().Be(2);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 19, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 36, 0));
            paused.Last().start.Should().Be(new DateTime(2021, 9, 22, 14, 47, 0));
            paused.Last().end.Should().Be(new DateTime(2021, 9, 22, 15, 01, 0));
        }

        [Fact]
        public void SixTimesWorkTimePauseTimeWithCurrent()
        {
            var systemTime = new MockSystemTime(new DateTime(2021, 09, 22, 16, 00, 0));

            var sut = new WorkTimeService(new NullLogger<WorkTimeService>(), new FeiertagService(), _cfg, systemTime);

            var data = new List<CheckInItem>() {
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 6, 33, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 19, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 11, 36, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 14, 47, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 01, 0)},
                new CheckInItem() { CheckInDate = new DateTime(2021, 09, 22), CheckinTime = new DateTime(2021, 09, 22, 15, 37, 0)}
            };

            var (worked, paused) = sut.PrepareCheckins(data, true);

            worked.Count.Should().Be(3);
            worked[0].start.Should().Be(new DateTime(2021, 9, 22, 6, 33, 0));
            worked[0].end.Should().Be(new DateTime(2021, 9, 22, 11, 19, 0));
            worked[1].start.Should().Be(new DateTime(2021, 9, 22, 11, 36, 0));
            worked[1].end.Should().Be(new DateTime(2021, 9, 22, 14, 47, 0));
            worked[2].start.Should().Be(new DateTime(2021, 9, 22, 15, 01, 0));
            worked[2].end.Should().Be(new DateTime(2021, 9, 22, 15, 37, 0));
            paused.Count.Should().Be(2);
            paused.First().start.Should().Be(new DateTime(2021, 9, 22, 11, 19, 0));
            paused.First().end.Should().Be(new DateTime(2021, 9, 22, 11, 36, 0));
            paused.Last().start.Should().Be(new DateTime(2021, 9, 22, 14, 47, 0));
            paused.Last().end.Should().Be(new DateTime(2021, 9, 22, 15, 01, 0));
        }
    }
}
