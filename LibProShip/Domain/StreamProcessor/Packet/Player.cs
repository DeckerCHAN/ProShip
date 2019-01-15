namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Player
    {
        public Player(string name, int accountId)
        {
            this.Name = name;
            this.AccountId = accountId;
        }

        public string Name { get; }
        public int AccountId { get; }

        public static bool operator ==(Player p1, Player p2)
        {
            return p1?.Equals((object)p2) ?? ReferenceEquals(p2, null);
        }

        public static bool operator !=(Player p1, Player p2)
        {
            return !(p1 == p2);
        }

        private bool Equals(Player other)
        {
            return this.AccountId == other.AccountId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Player other && Equals(other);
        }

        public override int GetHashCode()
        {
            return AccountId;
        }
    }
}