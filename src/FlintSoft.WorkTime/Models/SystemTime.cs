using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintSoft.WorkTime.Models
{
    public interface ISystemTime
    {
        DateTime Now { get; }
    }
    public class SystemTime : ISystemTime
    {
        public DateTime Now => DateTime.Now;
    }
}
