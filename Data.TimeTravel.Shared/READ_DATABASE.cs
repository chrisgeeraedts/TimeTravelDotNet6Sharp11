using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.TimeTravel.Shared
{
    public static class READ_DATABASE<TInternal, TEnvelop>
    {
        public static void RESET()
        {
            InternalSalaries = new List<TInternal>();
            InternalSalaryEnvelops = new List<TEnvelop>();
        }

        public static List<TInternal> InternalSalaries { get; set; }
        public static List<TEnvelop> InternalSalaryEnvelops { get; set; }

    }
}
