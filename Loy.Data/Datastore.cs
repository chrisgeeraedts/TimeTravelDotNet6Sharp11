using Data.TimeTravel.Shared;
using System.Text.Json;

namespace Loy.Data
{
    public class SalaryDataStore : IDataStore
    {

        public WRITE_DATABASE_EVENT_TABLE SalaryAmountEvents { get; set; }
        public WRITE_DATABASE_EVENT_TABLE SalaryTypeEvents { get; set; }
        public WRITE_DATABASE_EVENT_TABLE SalaryDescriptionEvents { get; set; }
        public WRITE_DATABASE_EVENT_TABLE IsActiveDateTimeEvents { get; set; }

        public WRITE_DATABASE SalaryEvents { get; set; }

        public READ_DATABASE<InternalSalary, Envelop<InternalSalary>> InternalSalaries { get; set; }

        public void STORE_EVENTS()
        {
            new DataStoreHelper().STORE_EVENT(SalaryAmountEvents, nameof(SalaryAmountEvents));
            new DataStoreHelper().STORE_EVENT(SalaryTypeEvents, nameof(SalaryTypeEvents));
            new DataStoreHelper().STORE_EVENT(SalaryDescriptionEvents, nameof(SalaryDescriptionEvents));
            new DataStoreHelper().STORE_EVENT(IsActiveDateTimeEvents, nameof(IsActiveDateTimeEvents));
        }
    }

    public interface IDataStore
    {
        void STORE_EVENTS();
    }
    public class DataStoreHelper
    {
        private string basePath = @"c:\DATASTORE\";
        public void STORE_EVENT(WRITE_DATABASE_EVENT_TABLE table, string name)
        {
            string json = JsonSerializer.Serialize(table);
            File.WriteAllText(basePath + name, json);
        }

        public void RETRIEVE()
        {

        }
    }

}
