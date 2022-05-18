using System;

namespace Loy.Tests
{
    public class TimeTravelResult<T>
    {
        public DateTime PIT_SimulatedCurrentTime { get; set; }
        public DateTime VDT_DateTimeOfData { get; set; }

        public T Result { get; set; }
        public string Description { get; set; }
    }
}