namespace LibProShip.Domain.StreamProcessor.Packet
{
    public class Matrix3
    {
        public Matrix3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }


        public override string ToString()
        {
            return $"x:{this.X} y:{this.Y} z:{this.Z}";
        }
    }
}