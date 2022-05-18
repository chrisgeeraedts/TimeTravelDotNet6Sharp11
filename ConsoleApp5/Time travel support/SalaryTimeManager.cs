//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TimeTravelTest.Entities;

//namespace TimeTravelTest
//{
//    public class SalaryTimeManager : ITimeManager
//    {
//        private Event CreateEvent(Guid internalPrimaryKey, dynamic objectValue, string modifiedBy)
//        {
//            return new Event()
//            {
//                ForeignKey = internalPrimaryKey,
//                Value = objectValue,
//                ValueType = ((dynamic)objectValue).Unwrap().GetType(),
//                TechnicalCreationDateTime = DateTime.Now,
//                FunctionalStartDate = DateTime.Now,
//                ModifiedBy = modifiedBy
//            };
//        }

//        private Event CreateEvent(Guid internalPrimaryKey, dynamic objectValue, DateTime functionalStartDate, string modifiedBy)
//        {
//            return new Event()
//            {
//                ForeignKey = internalPrimaryKey,
//                Value = objectValue,
//                ValueType = ((dynamic)objectValue).Unwrap().GetType(),
//                TechnicalCreationDateTime = DateTime.Now,
//                FunctionalStartDate = functionalStartDate,
//                ModifiedBy = modifiedBy
//            };
//        }

//        private Event CreateEvent_OVERWRITE<T>(Guid internalPrimaryKey, dynamic objectValue, DateTime functionalStartDatetime, DateTime creationDateTime, string modifiedBy)
//        {
//            return new Event()
//            {
//                ForeignKey = internalPrimaryKey,
//                Value = objectValue,
//                ValueType = ((dynamic)objectValue).Unwrap().GetType(),
//                TechnicalCreationDateTime = creationDateTime,
//                FunctionalStartDate = functionalStartDatetime,
//                ModifiedBy = modifiedBy
//            };
//        }


//        private T GetValue<T>(Guid PrimaryKey, DateTime PointInTime, DateTime ValueDateTime, List<Event> eventList)
//        {
//            var allEventsOfEntity = eventList.Where(x => x.ForeignKey == PrimaryKey);

//            // get all events where creationdatetime <= PointInTime
//            var resultsStep1 = allEventsOfEntity.Where(x => x.TechnicalCreationDateTime <= PointInTime);
//            var resultsStep2 = resultsStep1.Where(x => x.FunctionalStartDate <= ValueDateTime);

//            // get all records that are starting at the last startdatetime
//            var highestStartdatetime = resultsStep2.Max(x => x.FunctionalStartDate);
//            var resultsStep3 = resultsStep2.Where(x => x.FunctionalStartDate == highestStartdatetime);

//            // get the last created event
//            var highestCreationdatetime = resultsStep3.Max(x => x.TechnicalCreationDateTime);
//            var resultsStep4 = resultsStep3.Where(x => x.TechnicalCreationDateTime == highestCreationdatetime);

//            // should only be one
//            if (resultsStep4.Count() > 1)
//            {
//                throw new Exception();
//            }

//            if (!resultsStep4.Any())
//            {
//                return default(T);
//            }

//            return (T)resultsStep4.First().Value;
//        }

//        private void UpdateREAD_DATABASE(Salary salary)
//        {
//            // on every update of events we calculate the read database record. This does not include future changes 

//            var envelop = READ_DATABASE.InternalSalaryEnvelops.FirstOrDefault(x => x.ExternalPrimaryKey == salary.ExternalPrimaryKey);
//            if (envelop != null)
//            {
//                var internalPrimaryKey = envelop.InternalPrimaryKey;
//                var internalSalaryCurrent = READ_DATABASE.InternalSalaries.FirstOrDefault(x => x.InternalPrimaryKey == internalPrimaryKey);

//                // update the read state
//                internalSalaryCurrent.SalaryAmount = GetValue<decimal>(internalPrimaryKey, DateTime.Now, DateTime.Now, WRITE_DATABASE.SalaryAmountEvents);
//                internalSalaryCurrent.SalaryDescription = GetValue<string>(internalPrimaryKey, DateTime.Now, DateTime.Now, WRITE_DATABASE.SalaryDescriptionEvents);
//                internalSalaryCurrent.SalaryType = GetValue<SalaryType>(internalPrimaryKey, DateTime.Now, DateTime.Now, WRITE_DATABASE.SalaryTypeEvents);

//                envelop.IsActive = GetValue<bool>(salary.ExternalPrimaryKey, DateTime.Now, DateTime.Now, WRITE_DATABASE.IsActiveDateTimeEvents);
//            }
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="PrimaryKey"></param>
//        /// <returns></returns>
//        public Salary READ(Guid PrimaryKey)
//        {
//            // simply read from READ table
//            var envelop = READ_DATABASE.InternalSalaryEnvelops.FirstOrDefault(x => x.ExternalPrimaryKey == PrimaryKey);
//            if (envelop != null)
//            {
//                var internalSalary = READ_DATABASE.InternalSalaries.FirstOrDefault(x => x.InternalPrimaryKey == envelop.InternalPrimaryKey);
//                return new Salary(internalSalary, envelop);
//            }
//            return null;
//        }

//        /// <summary>
//        /// Read the Salary object of a specific ID at a certain point in time WITH the knowledge of NOW
//        /// </summary>
//        /// <param name="PrimaryKey"></param>
//        /// <param name="ValueDateTime"></param>
//        /// <returns></returns>
//        public Salary READ(Guid PrimaryKey, DateTime ValueDateTime)
//        {
//            // get internalKey from primary key
//            var envelop = READ_DATABASE.InternalSalaryEnvelops.FirstOrDefault(x => x.ExternalPrimaryKey == PrimaryKey);
//            if (envelop != null)
//            {
//                var internalPrimaryKey = envelop.InternalPrimaryKey;

//                var internalResult = new InternalSalary(); 
//                internalResult.InternalPrimaryKey = internalPrimaryKey;

//                internalResult.SalaryAmount = GetValue<decimal>(internalPrimaryKey, DateTime.Now, ValueDateTime, WRITE_DATABASE.SalaryAmountEvents);
//                internalResult.SalaryDescription = GetValue<string>(internalPrimaryKey, DateTime.Now, ValueDateTime, WRITE_DATABASE.SalaryDescriptionEvents);
//                internalResult.SalaryType = GetValue<SalaryType>(internalPrimaryKey, DateTime.Now, ValueDateTime, WRITE_DATABASE.SalaryTypeEvents);

//                var envelopResult = new Envelop<InternalSalary>(internalResult);
//                envelopResult.IsActive = GetValue<bool>(PrimaryKey, DateTime.Now, ValueDateTime, WRITE_DATABASE.IsActiveDateTimeEvents);

//                var result = new Salary(internalResult, envelopResult);

//                return result;
//            }

//            return null;
//        }

//        /// <summary>
//        /// Read the Salary object of a specific ID at a certain point in time WITH the knowledge of THEN
//        /// /// </summary>
//        /// <param name="PrimaryKey"></param>
//        /// <param name="PointInTime"></param>
//        /// <param name="ValueDateTime"></param>
//        /// <returns></returns>
//        public Salary READ(Guid PrimaryKey, DateTime PointInTime, DateTime ValueDateTime)
//        {
//            var envelop = READ_DATABASE.InternalSalaryEnvelops.FirstOrDefault(x => x.ExternalPrimaryKey == PrimaryKey);
//            if (envelop != null)
//            {
//                var internalPrimaryKey = envelop.InternalPrimaryKey;
//                var internalResult = new InternalSalary();
//                internalResult.InternalPrimaryKey = internalPrimaryKey;

//                internalResult.SalaryAmount = GetValue<decimal>(internalPrimaryKey, PointInTime, ValueDateTime, WRITE_DATABASE.SalaryAmountEvents);
//                internalResult.SalaryDescription = GetValue<string>(internalPrimaryKey, PointInTime, ValueDateTime, WRITE_DATABASE.SalaryDescriptionEvents);
//                internalResult.SalaryType = GetValue<SalaryType>(internalPrimaryKey, PointInTime, ValueDateTime, WRITE_DATABASE.SalaryTypeEvents);

//                var envelopResult = new Envelop<InternalSalary>(internalResult);
//                envelopResult.IsActive = GetValue<bool>(PrimaryKey, PointInTime, ValueDateTime, WRITE_DATABASE.IsActiveDateTimeEvents);

//                var result = new Salary(internalResult, envelopResult);

//                return result;
//            }

//            return null;
//        }

//        public void CREATE(Salary salary, string modifiedBy)
//        {
//            // go through all attributes and insert events
//            WRITE_DATABASE.InsertAmountEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryAmount, modifiedBy));
//            WRITE_DATABASE.InsertDescriptionEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryDescription, modifiedBy));
//            WRITE_DATABASE.InsertTypeEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryType, modifiedBy));

//            // create envelop events
//            WRITE_DATABASE.InsertActiveEvent(CreateEvent(salary.ExternalPrimaryKey, true, modifiedBy));

//            // create READ entity
//            READ_DATABASE.InternalSalaries.Add(salary.InternalSalary);
//            READ_DATABASE.InternalSalaryEnvelops.Add(salary.EnvelopSalary);
//        }

//        /// <summary>
//        /// Creates a salary that has a historical of future startdate
//        /// </summary>
//        /// <param name="salary"></param>
//        /// <param name="ValueDateTime"></param>
//        /// <param name="modifiedBy"></param>
//        public void CREATE(Salary salary, DateTime ValueDateTime, string modifiedBy)
//        {

//            // go through all attributes and insert events
//            WRITE_DATABASE.InsertAmountEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryAmount, ValueDateTime, modifiedBy));
//            WRITE_DATABASE.InsertDescriptionEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryDescription, ValueDateTime, modifiedBy));
//            WRITE_DATABASE.InsertTypeEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryType, ValueDateTime, modifiedBy));

//            // create envelop events
//            WRITE_DATABASE.InsertActiveEvent(CreateEvent(salary.ExternalPrimaryKey, true, ValueDateTime, modifiedBy));

//            // create READ entity
//            READ_DATABASE.InternalSalaries.Add(salary.InternalSalary);
//            READ_DATABASE.InternalSalaryEnvelops.Add(salary.EnvelopSalary);
//        }

//        public void CREATE_OVERWRITE(Salary salary, DateTime ValueDateTime, DateTime CreationDateTime, string modifiedBy)
//        {

//            // go through all attributes and insert events
//            //WRITE_DATABASE.InsertAmountEvent(CreateEvent_OVERWRITE(salary.InternalPrimaryKey, salary.SalaryAmount, ValueDateTime, CreationDateTime, modifiedBy));
//            //WRITE_DATABASE.InsertDescriptionEvent(CreateEvent_OVERWRITE(salary.InternalPrimaryKey, salary.SalaryDescription, ValueDateTime, CreationDateTime, modifiedBy));
//            //WRITE_DATABASE.InsertTypeEvent(CreateEvent_OVERWRITE(salary.InternalPrimaryKey, salary.SalaryType, ValueDateTime, CreationDateTime, modifiedBy));

//            //// create envelop events
//            //WRITE_DATABASE.InsertActiveEvent(CreateEvent_OVERWRITE(salary.ExternalPrimaryKey, true, ValueDateTime, CreationDateTime, modifiedBy));

//            //// create READ entity
//            //READ_DATABASE.InternalSalaries.Add(salary.InternalSalary);
//            //READ_DATABASE.InternalSalaryEnvelops.Add(salary.EnvelopSalary);
//        }

//        /// <summary>
//        /// Updates the current salary
//        /// </summary>
//        /// <param name="salary"></param>
//        /// <param name="modifiedBy"></param>
//        public void UPDATE(Salary salary, string modifiedBy)
//        {
//            // go through all attributes and insert events
//            WRITE_DATABASE.InsertAmountEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryAmount, modifiedBy));
//            WRITE_DATABASE.InsertDescriptionEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryDescription, modifiedBy));
//            WRITE_DATABASE.InsertTypeEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryType, modifiedBy));

//            UpdateREAD_DATABASE(salary);
//        }

//        /// <summary>
//        /// updates the salary at a given point in time
//        /// </summary>
//        /// <param name="salary"></param>
//        /// <param name="ValueDateTime"></param>
//        /// <param name="modifiedBy"></param>
//        public void UPDATE(Salary salary, DateTime ValueDateTime, string modifiedBy)
//        {
//            // go through all attributes and insert events
//            WRITE_DATABASE.InsertAmountEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryAmount, ValueDateTime, modifiedBy));
//            WRITE_DATABASE.InsertDescriptionEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryDescription, ValueDateTime, modifiedBy));
//            WRITE_DATABASE.InsertTypeEvent(CreateEvent(salary.InternalPrimaryKey, salary.SalaryType, ValueDateTime, modifiedBy));

//            UpdateREAD_DATABASE(salary);
//        }

//        /// <summary>
//        /// Delete the salary object
//        /// </summary>
//        /// <param name="salary"></param>
//        public void SOFT_DELETE(Salary salary, string modifiedBy)
//        {
//            // Set active datetime of salary envelop to the current datetime
//            //WRITE_DATABASE.IsActiveDateTimeEvents.Add(CreateEvent<bool>(salary.ExternalPrimaryKey, false, modifiedBy));
//        }

//        /// <summary>
//        /// Delete the salary object at a certain point in time
//        /// </summary>
//        /// <param name="salary"></param>
//        /// <param name="ValueDateTime"></param>
//        public void SOFT_DELETE(Salary salary, DateTime ValueDateTime, string modifiedBy)
//        {
//            // Set active datetime of salary envelop to the current datetime
//            //WRITE_DATABASE.IsActiveDateTimeEvents.Add(CreateEvent<bool>(salary.ExternalPrimaryKey, false, ValueDateTime, modifiedBy));
//        }

//        public void HARD_DELETE(Salary salary, string modifiedBy)
//        {
//            var envelop = READ_DATABASE.InternalSalaryEnvelops.FirstOrDefault(x => x.ExternalPrimaryKey == salary.ExternalPrimaryKey);
//            if (envelop != null)
//            {
//                var internalPrimaryKey = envelop.InternalPrimaryKey;

//                // remove the events regarding this entity
//                WRITE_DATABASE.DeleteAmountEvents(internalPrimaryKey);
//                WRITE_DATABASE.DeleteDescriptionEvents(internalPrimaryKey);
//                WRITE_DATABASE.DeleteTypeEvents(internalPrimaryKey);

//                var internalSalary = READ_DATABASE.InternalSalaries.FirstOrDefault(x => x.InternalPrimaryKey == internalPrimaryKey);
//                if (internalSalary != null)
//                {   
//                    // remove the read state
//                    READ_DATABASE.InternalSalaries.Remove(internalSalary);
//                }
//                else
//                {
//                    throw new Exception();
//                }
//            }
//            else
//            {
//                throw new Exception();
//            }

//            // Set active datetime of salary envelop to the current datetime
//            //WRITE_DATABASE.IsActiveDateTimeEvents.Add(CreateEvent<bool>(salary.ExternalPrimaryKey, false, modifiedBy));
//        }
//    }
//}