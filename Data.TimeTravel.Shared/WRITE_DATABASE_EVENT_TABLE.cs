namespace Data.TimeTravel.Shared
{
    public class WRITE_DATABASE_EVENT_TABLE
    {
        public WRITE_DATABASE_EVENT_TABLE(Type eventValueType, bool isEnvelopEvent = false)
        {
            Events = new List<Event>();
            EventValueType = eventValueType;
            IsEnvelopEvent = isEnvelopEvent;
        }

        public Type EventValueType { get; set; }
        public List<Event> Events { get; set; }
        public bool IsEnvelopEvent { get; set; }


        public void DELETE_HARD(Guid primaryKey)
        {
            var eventsToDelete = Events.Where(x => x.ForeignKey == primaryKey);
            foreach (var _event in eventsToDelete)
            {
                Events.Remove(_event);
            }
        }
    }
}
