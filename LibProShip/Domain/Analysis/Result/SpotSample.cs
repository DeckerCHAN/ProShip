namespace LibProShip.Domain.Analysis.Result
{
    public sealed class PointSample : ValueObject
    {
        public PointSample(string title, double orbit, double centralDistance, Color color)
        {
            Title = title;
            Orbit = orbit;
            CentralDistance = centralDistance;
            Color = color;
        }

        public string Title { get; }
        public double Orbit { get; }
        public double CentralDistance { get; }
        public Color Color { get; }
    }

    public sealed class SpotSample : ValueObject
    {
        public SpotSample(string title, double value, Color color, double centralDistance, double orbit)
        {
            Title = title;
            Value = value;
            Color = color;
            CentralDistance = centralDistance;
            Orbit = orbit;
        }


        public SpotSample()
        {
            //For ORM
        }

        public string Title { get; }
        public double Orbit { get; }
        public double CentralDistance { get; }
        public double Value { get; }
        public Color Color { get; }
    }
}