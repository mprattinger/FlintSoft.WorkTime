using FlintSoft.WorkTime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintSoft.WorkTime.Tests
{
    public class MockSystemTime : ISystemTime
    {
        private readonly DateTime _mockNow;

        public DateTime Now => _mockNow;

        public MockSystemTime(DateTime mockNow)
        {
            _mockNow = mockNow;
        }
    }
}
