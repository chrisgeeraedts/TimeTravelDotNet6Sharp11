using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeTravelTest.Entities;

namespace TimeTravelTest.Time_travel_support
{

    public interface IConstructor<TInternalEntity, TEnvelopEntityy>
    {
        void Constructor(TInternalEntity internalSalary, TEnvelopEntityy envelop);

        TInternalEntity InternalContainer { get; set; }
        TEnvelopEntityy EnvelopContainer { get; set; }
    }



    public class TimeTravelManager<TEntity, TInternalEntity, TEnvelopEntityy> 
        : ITimeManager<TEntity, TInternalEntity, TEnvelopEntityy>
        where TEntity : IConstructor<TInternalEntity, TEnvelopEntityy>
        where TEnvelopEntityy : Envelop<TInternalEntity> 
        where TInternalEntity : IBaseEntity
    {
        private List<TInternalEntity> _internalTableRef;
        private List<TEnvelopEntityy> _envelopTableRef;
        private Dictionary<string, WRITE_DATABASE_EVENT_TABLE> _attributeAndEventTableMap;


        public TimeTravelManager(
             List<TInternalEntity> internalTableRef, 
             List<TEnvelopEntityy> envelopTableRef
            )
        {
            _internalTableRef = internalTableRef;
            _envelopTableRef = envelopTableRef;
            _attributeAndEventTableMap = new Dictionary<string, WRITE_DATABASE_EVENT_TABLE>();

            InitMappingTable();
        }

        private void InitMappingTable()
        {
            // get all fields of the salary object
            var fields = ((System.Reflection.TypeInfo)typeof(TInternalEntity)).DeclaredProperties;
            foreach (var field in fields)
            {
                // get the attribute to get the event table from
                var attributes = field.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
                foreach (var attribute in attributes)
                {
                    var att = (DatabaseFieldAttribute)attribute;
                    var listName = att.ListName;

                    // find list in WRITE database
                    foreach (var prop in typeof(WRITE_DATABASE).GetProperties())
                    {
                        if (prop != null && prop.Name == listName)
                        {
                            WRITE_DATABASE_EVENT_TABLE? table = prop.GetValue(null) as WRITE_DATABASE_EVENT_TABLE;
                            if (table != null)
                            { 
                                _attributeAndEventTableMap.Add(att.AttributeName, table); 
                            }
                            
                        }
                    }
                }
            }
        }

        public void CREATE(TEntity salary, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public void CREATE(TEntity salary, DateTime ValueDateTime, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public void CREATE_OVERWRITE(TEntity salary, DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy)
        {

            // 1. get all fields from the salary entity (internal and envelop)
            var _internal = salary.InternalContainer;
            var _envelop = salary.EnvelopContainer;

            // 2. get the fields from the internal salary (amount, etc) and create events for them
            WriteFieldEvents(ValueDateTime, CreationDateTime, modifiedBy, _internal, _envelop);

            // 3. create the active event from the external id
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(_envelop.ExternalPrimaryKey, ValueDateTime, CreationDateTime, modifiedBy, _envelop, typeof(bool), true));

            // 4. create READ entity
            _internalTableRef.Add(_internal);
            _envelopTableRef.Add(_envelop);
        }

        

        public TEntity READ(Guid PrimaryKey)
        {

            // simply read from READ table
            var envelop = _envelopTableRef.FirstOrDefault(x => x.ExternalPrimaryKey == PrimaryKey);
            if (envelop != null)
            {
                TInternalEntity internalSalary = _internalTableRef.FirstOrDefault(x => x.InternalPrimaryKey == envelop.InternalPrimaryKey);

                TEntity result = (TEntity)Activator.CreateInstance(typeof(TEntity), internalSalary, envelop);

                return result;


            }
            return default(TEntity);
        }

        public TEntity READ(Guid PrimaryKey, DateTime ValueDateTime)
        {
            // simply read from READ table
            var envelop = _envelopTableRef.FirstOrDefault(x => x.ExternalPrimaryKey == PrimaryKey);
            if (envelop != null)
            {
                TInternalEntity internalSalary = _internalTableRef.FirstOrDefault(x => x.InternalPrimaryKey == envelop.InternalPrimaryKey);

                TInternalEntity internalResult = (TInternalEntity)Activator.CreateInstance(typeof(TInternalEntity));
                if (internalResult != null)
                {
                    // set primary key
                    internalResult.InternalPrimaryKey = envelop.InternalPrimaryKey;

                    // set fields
                    var fields = ((System.Reflection.TypeInfo)typeof(TInternalEntity)).DeclaredProperties;
                    foreach (var field in fields)
                    {
                        WRITE_DATABASE_EVENT_TABLE _eventTable = null;
                        
                        var attributes = field.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
                        if (attributes.Any())
                        {
                            var att = (DatabaseFieldAttribute)attributes[0];
                            if (_attributeAndEventTableMap.ContainsKey(att.AttributeName))
                            {
                                _eventTable = _attributeAndEventTableMap[att.AttributeName];

                                dynamic value = GetValue(envelop.InternalPrimaryKey, DateTime.Now, ValueDateTime, _eventTable.Events, _eventTable.EventValueType);
                                field.SetValue(internalResult, value);
                            }
                        }
                    }
                }

                TEnvelopEntityy envelopResult = (TEnvelopEntityy)Activator.CreateInstance(typeof(TEnvelopEntityy), internalResult);
                envelopResult.IsActive = GetValue(envelop.ExternalPrimaryKey, DateTime.Now, ValueDateTime, WRITE_DATABASE.IsActiveDateTimeEvents.Events, WRITE_DATABASE.IsActiveDateTimeEvents.EventValueType);

                TEntity result = (TEntity)Activator.CreateInstance(typeof(TEntity), internalResult, envelopResult);

                return result;
            }
            
            return default(TEntity);
        }

        //private dynamic GetValue(Guid internalPrimaryKey, PropertyInfo field, DateTime now, DateTime valueDateTime, WRITE_DATABASE_EVENT_TABLE eventTable)
        //{
        //    throw new NotImplementedException();
        //}

        public TEntity READ(Guid PrimaryKey, DateTime PointInTime, DateTime ValueDateTime)
        {
            throw new NotImplementedException();
        }

        public void SOFT_DELETE(TEntity salary, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public void SOFT_DELETE(TEntity salary, DateTime ValueDateTime, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public void HARD_DELETE(TEntity salary, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public void UPDATE(TEntity salary, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public void UPDATE(TEntity salary, DateTime ValueDateTime, string modifiedBy)
        {
            throw new NotImplementedException();
        }



      

        private void WriteFieldEvents(DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy, TInternalEntity _internal, TEnvelopEntityy _envelop)
        {
            var fields = ((System.Reflection.TypeInfo)typeof(TInternalEntity)).DeclaredProperties;
            foreach (var field in fields)
            {
                WRITE_DATABASE_EVENT_TABLE _eventTable = null;
                dynamic value = field.GetValue(_internal);
                var attributes = field.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
                if (attributes.Any())
                {
                    var att = (DatabaseFieldAttribute)attributes[0];
                    if (_attributeAndEventTableMap.ContainsKey(att.AttributeName))
                    {
                        _eventTable = _attributeAndEventTableMap[att.AttributeName];
                        WRITE_DATABASE.InsertEvent(_eventTable, CreateEvent(_envelop.InternalPrimaryKey, ValueDateTime, CreationDateTime, modifiedBy, _envelop, field.GetType(), value));
                    }
                }
            }
        }

        private static Event CreateEvent(Guid primaryKey, DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy, TEnvelopEntityy _envelop, Type type, dynamic value)
        {
            return new Event()
            {
                ForeignKey = primaryKey,
                Value = value,
                ValueType = type,
                ModifiedBy = modifiedBy,
                FunctionalStartDate = ValueDateTime,
                TechnicalCreationDateTime = CreationDateTime
            };
        }


        private dynamic GetValue(Guid PrimaryKey, DateTime PointInTime, DateTime ValueDateTime, List<Event> eventList, Type type)
        {
            var allEventsOfEntity = eventList.Where(x => x.ForeignKey == PrimaryKey);

            // get all events where creationdatetime <= PointInTime
            var resultsStep1 = allEventsOfEntity.Where(x => x.TechnicalCreationDateTime <= PointInTime);
            var resultsStep2 = resultsStep1.Where(x => x.FunctionalStartDate <= ValueDateTime);

            // get all records that are starting at the last startdatetime
            if (resultsStep2.Any())
            {
                var highestStartdatetime = resultsStep2.Max(x => x.FunctionalStartDate);
                var resultsStep3 = resultsStep2.Where(x => x.FunctionalStartDate == highestStartdatetime);

                // get the last created event
                var highestCreationdatetime = resultsStep3.Max(x => x.TechnicalCreationDateTime);
                var resultsStep4 = resultsStep3.Where(x => x.TechnicalCreationDateTime == highestCreationdatetime);

                // should only be one
                if (resultsStep4.Count() > 1)
                {
                    throw new Exception();
                }

                if (!resultsStep4.Any())
                {
                    return null;
                }

                return resultsStep4.First().Value;
            }
            else
            {
                return null;
            }

        }


        private void UpdateREAD_DATABASE(Salary salary)
        {
            throw new NotImplementedException();
        }




    }
}
