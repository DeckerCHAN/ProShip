namespace LibProShip.Domain.Analysis.Result
{
    public sealed class Color : ValueObject
    {
        public static Color RED = new Color(255, 0, 0);
        public static Color GREEN = new Color(0, 255, 0);
        public static Color BLUE = new Color(0, 0, 255);

        public Color(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color()
        {
            //For ORM
        }

        public double R { get; }
        public double G { get; }
        public double B { get; }
    }
}