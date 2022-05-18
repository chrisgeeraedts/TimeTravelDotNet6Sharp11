using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTravelTest.Entities;

namespace TimeTravelTest.Scenarios
{
    public class TestScenarioOne : ITestScenario<Salary>
    {
        public TestScenarioOne()
        {
            // Prepare the simulated tables
            WRITE_DATABASE.RESET();
            READ_DATABASE<InternalSalary,Envelop <InternalSalary>>.RESET();
        }



        Time_travel_support.TimeTravelManager<Salary, InternalSalary, Envelop<InternalSalary>> timeManager;
        Guid primaryKey;
        public void Prepare()
        {
            timeManager = new Time_travel_support.TimeTravelManager<Salary, InternalSalary, Envelop<InternalSalary>>
                (
                READ_DATABASE<InternalSalary, Envelop<InternalSalary>>.InternalSalaries, 
                READ_DATABASE<InternalSalary, Envelop<InternalSalary>>.InternalSalaryEnvelops
                );



            //timeManager = new SalaryTimeManager();

            // Create salary object
            Console.WriteLine("### INSERTING NEW SALARY");
            var salary = new Salary(50000, SalaryType.SV_LOON, "SV loon uit aanlevering 2021");
            timeManager.CREATE_OVERWRITE(salary, new DateTime(2020, 1, 1), new DateTime(2020, 1, 1), "Chris eeraedts");
            Console.WriteLine(Environment.NewLine);

            //Console.WriteLine("### Opvoeren toekomstig salaris");
            //salary.SalaryAmount = 55000;
            //salary.SalaryDescription = "Toekomstig salaris vanaf 1-1-2021 (ten tijde van 21-2-2020)";
            //timeManager.CREATE_OVERWRITE(salary, new DateTime(2021, 1, 1), new DateTime(2020, 2, 21), "Chris Geeraedts");
            //Console.WriteLine(Environment.NewLine);

            //Console.WriteLine("### Verhogen huidig salaris");
            //salary.SalaryAmount = 51000;
            //salary.SalaryDescription = "Verhoging van huidig salaris";
            //timeManager.CREATE_OVERWRITE(salary, new DateTime(2021, 4, 21), new DateTime(2021, 4, 21), "Chris Geeraedts");


            primaryKey = salary.ExternalPrimaryKey;
            Console.WriteLine(Environment.NewLine);
        }

        public TimeTravelResults<Salary> Run()
        {
            var result = new TimeTravelResults<Salary>();

            result.results.Add(CreateResult("Data nu"));
            result.results.Add(CreateResult("Data eerst", new DateTime(2020, 1, 1)));

            //result.results.Add(CreateResult(DateTime.Now, new DateTime(2020, 1, 1), "Direct na opvoer"));
            //result.results.Add(CreateResult(DateTime.Now, new DateTime(2021, 4, 21), "Salaris update"));
            //result.results.Add(CreateResult(DateTime.Now, new DateTime(2021, 1, 1), "Ingang nieuw salaris"));

            return result;
        }

        private TimeTravelResult<Salary> CreateResult(DateTime PIT_SimulatedCurrentTime, DateTime VDT_DateTimeOfData, string description)
        {
            return new TimeTravelResult<Salary>()
            {
                PIT_SimulatedCurrentTime = PIT_SimulatedCurrentTime,
                VDT_DateTimeOfData = VDT_DateTimeOfData,
                Result = timeManager.READ(primaryKey, PIT_SimulatedCurrentTime, VDT_DateTimeOfData),
                Description = description
            };
        }

        private TimeTravelResult<Salary> CreateResult(string description)
        {
            return new TimeTravelResult<Salary>()
            {
                PIT_SimulatedCurrentTime = DateTime.Now,
                VDT_DateTimeOfData = DateTime.Now,
                Result = timeManager.READ(primaryKey),
                Description = description
            };
        }

        private TimeTravelResult<Salary> CreateResult(string description, DateTime VDT_DateTimeOfData)
        {
            return new TimeTravelResult<Salary>()
            {
                PIT_SimulatedCurrentTime = DateTime.Now,
                VDT_DateTimeOfData = VDT_DateTimeOfData,
                Result = timeManager.READ(primaryKey, VDT_DateTimeOfData),
                Description = description
            };
        }
    }
}
