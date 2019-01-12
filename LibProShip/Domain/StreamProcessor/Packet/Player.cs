namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Player
    {
        public Player(string name, int accountId, long shipId)
        {
            this.Name = name;
            this.AccountId = accountId;
            this.ShipId = shipId;
        }

        public string Name { get; }
        public int AccountId { get; }
        public long ShipId { get; }

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
            return string.Equals(this.Name, other.Name) && this.AccountId == other.AccountId && this.ShipId == other.ShipId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Player other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.AccountId;
                hashCode = (hashCode * 397) ^ this.ShipId.GetHashCode();
                return hashCode;
            }
        }
    }
}