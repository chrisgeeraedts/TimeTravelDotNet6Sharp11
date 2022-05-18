using Data.TimeTravel.Shared;

namespace Loy.Data
{
    public class EntityAndStoreMap<TEntity>
    {
        public Type EntityType { get; set; }
        public IDataStore Datastore { get; set; }
    }
}
