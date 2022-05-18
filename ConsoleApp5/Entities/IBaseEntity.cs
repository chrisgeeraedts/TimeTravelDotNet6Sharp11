using System;

namespace TimeTravelTest
{
    public interface IBaseEntity : IDataEntity
    {
        Guid InternalPrimaryKey { get; set; }
    }
}