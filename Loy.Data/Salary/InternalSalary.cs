using Data.TimeTravel.Shared;

namespace Loy.Data
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


        [DatabaseField("SalaryType", nameof(WRITE_DATABASE.SalaryTypeEvents), typeof(string))]
        public string SalaryType { get; set; }


        [DatabaseField("SalaryDescription", nameof(WRITE_DATABASE.SalaryDescriptionEvents), typeof(string))]
        public string SalaryDescription { get; set; }
    }
}