namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Matrix3
    {
        public Matrix3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }


        public override string ToString()
        {
            return $"x:{X} y:{Y} z:{Z}";
        }

        protected bool Equals(Matrix3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }


        public static bool operator ==(Matrix3 o1, Matrix3 o2)
        {
            return o1?.Equals((object) o2) ?? ReferenceEquals(o2, null);
        }

        public static bool operator !=(Matrix3 o1, Matrix3 o2)
        {
            return !(o1 == o2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Matrix3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
    }
}