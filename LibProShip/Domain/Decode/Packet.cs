namespace LibProShip.Domain.Decode
{
    public sealed class Packet : IValueObject<Packet>
    {
        public Packet(long time, dynamic data)
        {
            Time = time;
            Data = data;
        }

        public long Time { get; }
        public dynamic Data { get; }

        public bool SameAs(Packet other)
        {
            throw new System.NotImplementedException();
        }
    }
}