namespace LibProShip.Domain.StreamProcessor.Packet
{
    public sealed class Matrix3
    {
        public Matrix3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }


        public override string ToString()
        {
            return $"x:{this.X} y:{this.Y} z:{this.Z}";
        }

        protected bool Equals(Matrix3 other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z);
        }


        public static bool operator ==(Matrix3 o1, Matrix3 o2)
        {
            return o1?.Equals((object)o2) ?? object.ReferenceEquals(o2, null);
        }

        public static bool operator !=(Matrix3 o1, Matrix3 o2)
        {
            return !(o1 == o2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Matrix3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.X.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Z.GetHashCode();
                return hashCode;
            }
        }
    }
}