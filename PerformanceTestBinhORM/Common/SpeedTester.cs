//http://www.vcskicks.com/
using System;
using System.Diagnostics;

namespace Common
{
    public class SpeedTester
    {
        public delegate void MethodHandler();
        private int totalRunningTime;
        private double averageRunningTime;

        public int TotalRunningTime
        {
            get { return totalRunningTime; }
        }

        public double AverageRunningTime
        {
            get { return averageRunningTime; }
        }

        private MethodHandler method;

        public SpeedTester(MethodHandler methodToTest)
        {
            this.method = methodToTest;
        }

        public void RunTest()
        {
            RunTest(10); //default 10 trials
        }

        public void RunTest(int trials)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            for (int i = 0; i < trials; i++)
            {
                method.Invoke(); //run the method
            }
            watch.Stop();

            totalRunningTime = (int)watch.ElapsedMilliseconds; //total milliseconds
            averageRunningTime = (double)TotalRunningTime / trials; //total time over number of trials
        }
    }
}
