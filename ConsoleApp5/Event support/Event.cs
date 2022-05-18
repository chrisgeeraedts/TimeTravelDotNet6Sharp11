using System;

namespace TimeTravelTest
{
    public class Event
    {
        public Guid ForeignKey { get; set; }

        public object Value { get; set; }

        public Type ValueType { get; set; }

        public DateTime FunctionalStartDate { get; set; }

        public DateTime TechnicalCreationDateTime { get; set; }

        public string ModifiedBy { get; set; }
    }
}