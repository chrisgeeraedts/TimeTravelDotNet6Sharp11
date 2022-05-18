namespace TimeTravelTest.Entities
{
    public class WRITE_DATABASE_EVENT_TABLE
    {
        public WRITE_DATABASE_EVENT_TABLE(Type eventValueType)
        {
            Events = new List<Event>();
            EventValueType = eventValueType;
        }
        public Type EventValueType { get; set; }
        public List<Event> Events { get; set; }
    }
}
