using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTestBinhORM.Common
{
    public class TestResult
    {
        public string TestMethod { get; set; }
        public int RunTimes { get; set; }
        public double AverageRunningTime { get; set; }
        public double TotalRunningTime { get; set; }
        public double ADOAverageRunningTime { get; set; }
        public double ADOTotalRunningTime { get; set; }
    }
}
