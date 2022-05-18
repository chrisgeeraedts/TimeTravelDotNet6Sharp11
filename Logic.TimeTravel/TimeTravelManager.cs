using Data.TimeTravel.Shared;
using Logic.TimeTravel.Shared;
using System.Reflection;

namespace Logic.TimeTravel
{
    public class TimeTravelManager<TEntity, TInternalEntity, TEnvelopEntity> 
        : ITimeManager<TEntity, TInternalEntity, TEnvelopEntity>
        where TEntity : IConstructor<TInternalEntity, TEnvelopEntity>
        where TEnvelopEntity : Envelop<TInternalEntity> 
        where TInternalEntity : IBaseEntity
    {
        private readonly List<TInternalEntity> _internalTableRef;
        private readonly List<TEnvelopEntity> _envelopTableRef;
        private readonly Dictionary<string, WRITE_DATABASE_EVENT_TABLE> _attributeAndEventTableMap;


        public TimeTravelManager(
             List<TInternalEntity> internalTableRef, 
             List<TEnvelopEntity> envelopTableRef
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
            var fields = ((TypeInfo)typeof(TInternalEntity)).DeclaredProperties;
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

        #region CREATE

        public void CREATE(TEntity salary, string modifiedBy)
        {
            // 1. get all fields from the salary entity (internal and envelop)
            var _internal = salary.InternalContainer;
            var _envelop = salary.EnvelopContainer;

            // 2. get the fields from the internal salary (amount, etc) and create events for them
            WriteFieldEventsToWRITE(DateTime.Now, DateTime.Now, modifiedBy, _internal, _envelop);

            // 3. create the active event from the external id
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(_envelop.ExternalPrimaryKey, DateTime.Now, DateTime.Now, modifiedBy, _envelop, typeof(bool), true));

            // 4. create READ entity
            _internalTableRef.Add(_internal);
            _envelopTableRef.Add(_envelop);
        }

        public void CREATE(TEntity salary, DateTime ValueDateTime, string modifiedBy)
        {
            // 1. get all fields from the salary entity (internal and envelop)
            var _internal = salary.InternalContainer;
            var _envelop = salary.EnvelopContainer;

            // 2. get the fields from the internal salary (amount, etc) and create events for them
            WriteFieldEventsToWRITE(ValueDateTime, DateTime.Now, modifiedBy, _internal, _envelop);

            // 3. create the active event from the external id
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(_envelop.ExternalPrimaryKey, ValueDateTime, DateTime.Now, modifiedBy, _envelop, typeof(bool), true));

            // 4. create READ entity
            _internalTableRef.Add(_internal);
            _envelopTableRef.Add(_envelop);
        }

        public void CREATE_OVERWRITE(TEntity salary, DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy)
        {

            // 1. get all fields from the salary entity (internal and envelop)
            var _internal = salary.InternalContainer;
            var _envelop = salary.EnvelopContainer;

            // 2. get the fields from the internal salary (amount, etc) and create events for them
            WriteFieldEventsToWRITE(ValueDateTime, CreationDateTime, modifiedBy, _internal, _envelop);

            // 3. create the active event from the external id
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(_envelop.ExternalPrimaryKey, ValueDateTime, CreationDateTime, modifiedBy, _envelop, typeof(bool), true));

            // 4. create READ entity
            _internalTableRef.Add(_internal);
            _envelopTableRef.Add(_envelop);
        }

        #endregion

        #region READ

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

        public TEntity READ(Guid primaryKey, DateTime valueDateTime)
        {
            return READbaseOnWriteTable(primaryKey, valueDateTime, DateTime.Now);
        }

        public TEntity READ(Guid primaryKey, DateTime pointInTime, DateTime valueDateTime)
        {
            return READbaseOnWriteTable(primaryKey, valueDateTime, pointInTime);
        }

        private TInternalEntity CreateInternalEntityAndFillInternalFields(TEnvelopEntity envelop, DateTime PointInTime, DateTime ValueDateTime)
        {
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

                            dynamic value = GetValue(envelop.InternalPrimaryKey, PointInTime, ValueDateTime, _eventTable.Events, _eventTable.EventValueType);
                            field.SetValue(internalResult, value);
                        }
                    }
                }
            }

            return internalResult;
        }


        private TEntity READbaseOnWriteTable(Guid primaryKey, DateTime valueDateTime, DateTime pointInTime)
        {
            // simply read from READ table
            var envelop = _envelopTableRef.FirstOrDefault(x => x.ExternalPrimaryKey == primaryKey);
            if (envelop != null)
            {
                // create a new internal entity and fill its fields based on the given datetimes
                TInternalEntity internalResult = CreateInternalEntityAndFillInternalFields(envelop, pointInTime, valueDateTime);

                // create a envelop from the internal result
                TEnvelopEntity envelopResult = (TEnvelopEntity)Activator.CreateInstance(typeof(TEnvelopEntity), internalResult);

                // set the active field correct
                envelopResult.IsActive = GetValue(envelop.ExternalPrimaryKey, pointInTime, valueDateTime, WRITE_DATABASE.IsActiveDateTimeEvents.Events, WRITE_DATABASE.IsActiveDateTimeEvents.EventValueType);

                // create a container result entity and return
                TEntity result = (TEntity)Activator.CreateInstance(typeof(TEntity), internalResult, envelopResult);
                return result;
            }

            return default(TEntity);
        }


        #endregion

        #region UPDATE

        public void UPDATE(TEntity salary, string modifiedBy)
        {
            // 1. get all fields from the salary entity (internal and envelop)
            var _internal = salary.InternalContainer;
            var _envelop = salary.EnvelopContainer;

            // 2. get the fields from the internal salary (amount, etc) and create events for them
            WriteFieldEventsToWRITE(DateTime.Now, DateTime.Now, modifiedBy, _internal, _envelop);

            UpdateREAD_DATABASE(salary, _envelop, DateTime.Now);
        }

        public void UPDATE(TEntity salary, DateTime valueDateTime, string modifiedBy)
        {
            // 1. get all fields from the salary entity (internal and envelop)
            var _internal = salary.InternalContainer;
            var _envelop = salary.EnvelopContainer;

            // 2. get the fields from the internal salary (amount, etc) and create events for them
            WriteFieldEventsToWRITE(valueDateTime, DateTime.Now, modifiedBy, _internal, _envelop);

            UpdateREAD_DATABASE(salary, _envelop, valueDateTime);
        }

        // on every update of events we calculate the read database record. This does not include future changes 

        private void UpdateREAD_DATABASE(TEntity salary, TEnvelopEntity envelop, DateTime valueDateTime)
        {
            // on every update of events we calculate the read database record. This does not include future changes 

            if (envelop != null && //ensure we actually have a existing primary key
                valueDateTime <= DateTime.Now) //ensure we do not have a change in the future that we update the read table with
            { 
                WriteFieldEventsToREAD(salary.InternalContainer, salary.EnvelopContainer);
            }
        }

        #endregion

        #region DELETE

        public void SOFT_DELETE(TEntity salary, string modifiedBy)
        {
            // Set active datetime of salary envelop to the current datetime
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(
                salary.EnvelopContainer.ExternalPrimaryKey,
                DateTime.Now,
                DateTime.Now,
                modifiedBy,
                salary.EnvelopContainer,
                typeof(bool), false));
        }

        public void SOFT_DELETE(TEntity salary, DateTime ValueDateTime, string modifiedBy)
        {
            // Set active datetime of salary envelop to the current datetime
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(
                salary.EnvelopContainer.ExternalPrimaryKey,
                ValueDateTime,
                DateTime.Now,
                modifiedBy,
                salary.EnvelopContainer,
                typeof(bool), false));
        }

        public void HARD_DELETE(TEntity salary, string modifiedBy)
        {
            // Find events and remove
            foreach (var tableMapEntry in _attributeAndEventTableMap.Where(x => !x.Value.IsEnvelopEvent))
            {
                tableMapEntry.Value.DELETE_HARD(salary.EnvelopContainer.InternalPrimaryKey);
            }

            // Find read and remove internal entity
            var internalEntityToRemove = _internalTableRef.FirstOrDefault(x => x.InternalPrimaryKey == salary.InternalContainer.InternalPrimaryKey);
            if (internalEntityToRemove != null)
            {
                _internalTableRef.Remove(internalEntityToRemove);
            }

            //TODO: Also remove envelop?

            // Set active datetime of salary envelop to the current datetime
            WRITE_DATABASE.InsertEvent(WRITE_DATABASE.IsActiveDateTimeEvents, CreateEvent(
                salary.EnvelopContainer.ExternalPrimaryKey, 
                DateTime.Now, 
                DateTime.Now, 
                modifiedBy, 
                salary.EnvelopContainer, 
                typeof(bool), false));
        }

        #endregion

        #region support

        private void WriteFieldEventsToWRITE(DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy, TInternalEntity _internal, TEnvelopEntity _envelop)
        {
            var fields = ((TypeInfo)typeof(TInternalEntity)).DeclaredProperties;
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

        private void WriteFieldEventsToREAD(TInternalEntity _internal, TEnvelopEntity _envelop)
        {
            var fields = ((TypeInfo)typeof(TInternalEntity)).DeclaredProperties;

            int indexInternal = _internalTableRef.FindIndex(s => s.InternalPrimaryKey == _internal.InternalPrimaryKey);
            _internalTableRef[indexInternal] = _internal;

            int indexEnvelop = _envelopTableRef.FindIndex(s => s.InternalPrimaryKey == _internal.InternalPrimaryKey);
            _envelopTableRef[indexEnvelop] = _envelop;
        }

        private static Event CreateEvent(Guid primaryKey, DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy, TEnvelopEntity _envelop, Type type, dynamic value)
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

        #endregion
    }
}
