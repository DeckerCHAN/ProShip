namespace LibProShip.Domain.Analysis.Result
{
    public sealed class Color : ValueObject
    {
        public Color(double r, double g, double b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public Color()
        {
            //For ORM
        }

        public double R { private set; get; }
        public double G { private set; get; }
        public double B { private set; get; }
        public static Color RED = new Color(255, 0, 0);
        public static Color GREEN = new Color(0, 255, 0);
        public static Color BLUE = new Color(0, 0, 255);
    }
}