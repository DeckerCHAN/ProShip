namespace LibProShip.Domain.Decode
{
    public sealed class Packet : ValueObject<Packet>
    {
        public Packet(long id, long time, dynamic data)
        {
            this.Id = id;
            this.Time = time;
            this.Data = data;
        }

        public Packet()
        {
        }

        public long Time { get; }
        public dynamic Data { get; }

        public bool SameAs(Packet other)
        {
            throw new System.NotImplementedException();
        }
    }
}