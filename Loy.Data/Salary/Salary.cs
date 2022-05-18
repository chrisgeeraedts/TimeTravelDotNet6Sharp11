using Data.TimeTravel.Shared;

namespace Loy.Data
{
    public class Salary : ISalary, IConstructor<InternalSalary, Envelop<InternalSalary>>, IEntity
    {

        public Salary(decimal salaryAmount, SalaryType salaryType, string salaryDescription)
        {
            InternalContainer = new InternalSalary();
            SalaryAmount = salaryAmount;
            SalaryType = salaryType;
            SalaryDescription = salaryDescription;

            EnvelopContainer = new Envelop<InternalSalary>(InternalContainer);
            EnvelopContainer.IsActive = true;
        }

        public Salary(decimal salaryAmount, SalaryType salaryType, string salaryDescription, Guid primaryKey, Guid internalPrimaryKey)
        {
            InternalContainer = new InternalSalary();
            SalaryAmount = salaryAmount;
            SalaryType = salaryType;
            SalaryDescription = salaryDescription;

            EnvelopContainer = new Envelop<InternalSalary>(InternalContainer);
            EnvelopContainer.ExternalPrimaryKey = primaryKey;
            InternalContainer.InternalPrimaryKey = internalPrimaryKey;
            EnvelopContainer.IsActive = true;
        }

        public Salary(InternalSalary internalSalary)
        {
            EnvelopContainer = new Envelop<InternalSalary>(internalSalary);
            EnvelopContainer.IsActive = true;
            InternalContainer = internalSalary;
        }

        public Salary(InternalSalary internalSalary, Envelop<InternalSalary> envelop)
        {
            Constructor(internalSalary, envelop);
        }

        public Envelop<InternalSalary> EnvelopContainer { get; set; }
        public InternalSalary InternalContainer { get; set; }
        public Guid ExternalPrimaryKey { get => EnvelopContainer.ExternalPrimaryKey; }
        public Guid InternalPrimaryKey { get => InternalContainer.InternalPrimaryKey; }
        public decimal SalaryAmount { get => InternalContainer.SalaryAmount; set => InternalContainer.SalaryAmount = value; }
        public SalaryType SalaryType { get => (SalaryType)Enum.Parse(typeof(SalaryType), InternalContainer.SalaryType); set => InternalContainer.SalaryType = value.ToString(); }
        public string SalaryDescription { get => InternalContainer.SalaryDescription; set => InternalContainer.SalaryDescription = value; }

        public bool IsActive { get => EnvelopContainer.IsActive; set => EnvelopContainer.IsActive = value; }



        public void Constructor(InternalSalary internalSalary, Envelop<InternalSalary> envelop)
        {
            EnvelopContainer = envelop;
            InternalContainer = internalSalary;
        }
    }
}