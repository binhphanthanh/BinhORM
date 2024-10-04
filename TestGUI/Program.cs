using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WindowsApplication1;

namespace TestGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //MessageBox.Show(null, "Avatar quá\n\"khủng\" không\nhiển thị nổi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //SpeedTester t = new SpeedTester(Test);
            //t.RunTest(1);
            //Console.WriteLine(t.AverageRunningTime);
        }

        //public static void Test()
        //{
        //    string sql = @"select new Animal (con, meo) from abc join (select ds da)";
        //    Regex rx = new Regex(@"\s+new\s+(.|[\n\r])+\((.|[\n\r])+\)\s*");
        //    Match m = rx.Match(sql);
        //    if (m != null && m.Success)
        //    {
        //        int newIdx = sql.IndexOf(" new ");
        //        int leftBracketIdx = sql.IndexOf('(', newIdx);
        //        sql = sql.Remove(newIdx+1, leftBracketIdx - newIdx );

        //        int rightBracketIdx = sql.IndexOf(')', newIdx);
        //        sql = sql.Remove(rightBracketIdx, 1);
        //    }
        //    Console.WriteLine(sql);
        //}
    }
}