using System;

namespace TimeTravelTest
{
    public interface ITimeManager
    {
        Salary READ(Guid PrimaryKey);
        Salary READ(Guid PrimaryKey, DateTime ValueDateTime);
        Salary READ(Guid PrimaryKey, DateTime PointInTime, DateTime ValueDateTime);
        void CREATE(Salary salary, string modifiedBy);
        void CREATE(Salary salary, DateTime ValueDateTime, string modifiedBy);
        void UPDATE(Salary salary, string modifiedBy);
        void UPDATE(Salary salary, DateTime ValueDateTime, string modifiedBy);
        void SOFT_DELETE(Salary salary, string modifiedBy);
        void SOFT_DELETE(Salary salary, DateTime ValueDateTime, string modifiedBy);
        void HARD_DELETE(Salary salary, string modifiedBy);

    }

    public interface ITimeManager<TSalary, TInternalSalary, TEnvelopSalary>
    {
        TSalary READ(Guid PrimaryKey);
        TSalary READ(Guid PrimaryKey, DateTime ValueDateTime);
        TSalary READ(Guid PrimaryKey, DateTime PointInTime, DateTime ValueDateTime);
        void CREATE(TSalary salary, string modifiedBy);
        void CREATE(TSalary salary, DateTime ValueDateTime, string modifiedBy);
        void UPDATE(TSalary salary, string modifiedBy);
        void UPDATE(TSalary salary, DateTime ValueDateTime, string modifiedBy);
        void SOFT_DELETE(TSalary salary, string modifiedBy);
        void SOFT_DELETE(TSalary salary, DateTime ValueDateTime, string modifiedBy);
        void HARD_DELETE(TSalary salary, string modifiedBy);

    }
}