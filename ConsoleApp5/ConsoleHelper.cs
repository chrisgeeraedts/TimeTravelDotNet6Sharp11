using Loy.Data;
using Loy.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    internal static class ConsoleHelper
    {
        internal static void WriteResult(TimeTravelResult<Salary> result)
        {
            Console.WriteLine(string.Format("CURRENT TIME: {0}", result.PIT_SimulatedCurrentTime));
            Console.WriteLine(string.Format("TIME OF DATA: {0}", result.VDT_DateTimeOfData));

            Console.WriteLine(string.Format("{0} | {1} | {2}",
                result.Result.SalaryAmount,
                result.Result.SalaryType,
                result.Result.SalaryDescription));
            Console.WriteLine(Environment.NewLine);
        }
    }
}
