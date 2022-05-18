using System;

namespace Data.TimeTravel.Shared
{
    public interface IEvent
    {
        Guid ForeignKey { get; set; }

        dynamic Value { get; set; }

        Type valueType { get; set; }

        DateTime FunctionalStartDate { get; set; }

        DateTime TechnicalCreationDateTime { get; set; }

        string ModifiedBy { get; set; }
    }
}