namespace Data.TimeTravel.Shared
{
    public interface IConstructor<TInternalEntity, TEnvelopEntityy>
    {
        void Constructor(TInternalEntity internalSalary, TEnvelopEntityy envelop);

        TInternalEntity InternalContainer { get; set; }
        TEnvelopEntityy EnvelopContainer { get; set; }
    }
}
