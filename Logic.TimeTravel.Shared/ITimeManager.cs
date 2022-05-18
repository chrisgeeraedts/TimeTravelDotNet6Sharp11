using System;

namespace Logic.TimeTravel.Shared
{

    public interface ITimeManager<TEntity, TInternalEntity, TEnvelopEntity>
    {
        TEntity READ(Guid PrimaryKey);
        TEntity READ(Guid primaryKey, DateTime valueDateTime);
        TEntity READ(Guid primaryKey, DateTime pointInTime, DateTime valueDateTime);
        void CREATE(TEntity salary, string modifiedBy);
        void CREATE(TEntity salary, DateTime ValueDateTime, string modifiedBy);
        void UPDATE(TEntity salary, string modifiedBy);
        void UPDATE(TEntity salary, DateTime ValueDateTime, string modifiedBy);
        void SOFT_DELETE(TEntity salary, string modifiedBy);
        void SOFT_DELETE(TEntity salary, DateTime ValueDateTime, string modifiedBy);
        void HARD_DELETE(TEntity salary, string modifiedBy);

    }
}