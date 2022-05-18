using System.Collections.Generic;

namespace Loy.Tests
{
    public class TimeTravelResults<T>
    {
        public TimeTravelResults()
        {
            results = new List<TimeTravelResult<T>>();
        }
        public List<TimeTravelResult<T>> results { get; set; }
    }
}