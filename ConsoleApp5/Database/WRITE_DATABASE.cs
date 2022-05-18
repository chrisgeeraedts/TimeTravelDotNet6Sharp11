using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTravelTest.Entities
{


    public static class WRITE_DATABASE
    {
        public static void RESET()
        {
            IsActiveDateTimeEvents = new WRITE_DATABASE_EVENT_TABLE(typeof(bool) );
            SalaryAmountEvents = new WRITE_DATABASE_EVENT_TABLE(typeof(decimal) );
            SalaryDescriptionEvents = new WRITE_DATABASE_EVENT_TABLE(typeof(string) );
            SalaryTypeEvents = new WRITE_DATABASE_EVENT_TABLE( typeof(SalaryType));
        }


        public static WRITE_DATABASE_EVENT_TABLE SalaryAmountEvents { get; set; }
        public static WRITE_DATABASE_EVENT_TABLE SalaryTypeEvents { get; set; }
        public static WRITE_DATABASE_EVENT_TABLE SalaryDescriptionEvents { get; set; }

        public static WRITE_DATABASE_EVENT_TABLE IsActiveDateTimeEvents { get; set; }


        public static void InsertEvent(WRITE_DATABASE_EVENT_TABLE eventTable, Event @event)
        {
            eventTable.Events.Add(@event);
        }

        public static void DeleteAllEvent(WRITE_DATABASE_EVENT_TABLE eventTable, Guid primaryKey)
        {
            var toDelete = eventTable.Events.Where(x => x.ForeignKey == primaryKey);
            foreach (var item in toDelete)
            {
                eventTable.Events.Remove(item);
            }
        }
    }
}
