namespace Data.TimeTravel.Shared
{
    public class Envelop<T> where T : IBaseEntity
    {
        public Envelop(T baseObject)
        {
            ExternalPrimaryKey = Guid.NewGuid();
            InternalPrimaryKey = baseObject.InternalPrimaryKey;
            InternalObject = baseObject;
        }

        public Guid ExternalPrimaryKey { get; set; }

        public Guid InternalPrimaryKey { get; set; }

        public T InternalObject { get; set; }

        public bool IsActive { get; set; }
    }
}