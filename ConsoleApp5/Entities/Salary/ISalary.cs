using System;

namespace TimeTravelTest
{
    public interface ISalary
    {
        Guid ExternalPrimaryKey { get; }
        Guid InternalPrimaryKey { get; }
        decimal SalaryAmount { get; set; }
        SalaryType SalaryType { get; set; }
        string SalaryDescription { get; set; }

        bool IsActive { get; set; }
    }
}