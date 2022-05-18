using System;

namespace Data.TimeTravel.Shared
{
    public interface IBaseEntity : IDataEntity
    {
        Guid InternalPrimaryKey { get; set; }
    }
}