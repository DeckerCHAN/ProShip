namespace LibProShip.Domain
{
    public interface IValueObject<in T>
    {
        bool SameAs(T other);
    }
}