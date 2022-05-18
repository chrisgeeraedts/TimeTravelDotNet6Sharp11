using System;
using TimeTravelTest.Entities;

namespace TimeTravelTest
{
    public class InternalSalary : IBaseEntity
    {
        public InternalSalary()
        {
            InternalPrimaryKey = Guid.NewGuid();
        }

        public Guid InternalPrimaryKey { get; set; }


        [DatabaseField("SalaryAmount", nameof(WRITE_DATABASE.SalaryAmountEvents), typeof(decimal))]
        public decimal SalaryAmount { get; set; }


        [DatabaseField("SalaryType", nameof(WRITE_DATABASE.SalaryTypeEvents), typeof(SalaryType))]
        public SalaryType SalaryType { get; set; }


        [DatabaseField("SalaryDescription", nameof(WRITE_DATABASE.SalaryDescriptionEvents), typeof(string))]
        public string SalaryDescription { get; set; }
    }
}