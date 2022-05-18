namespace Data.TimeTravel.Shared
{
    public class READ_DATABASE_TABLE
    {
        public READ_DATABASE_TABLE(Type entityType)
        {
            Records = new List<dynamic>();
            EntityType = entityType;
        }
        public List<dynamic> Records { get; set; }
        public Type EntityType { get; }
    }
}
