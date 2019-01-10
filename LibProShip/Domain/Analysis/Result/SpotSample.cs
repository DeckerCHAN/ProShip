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

        public string Title { get; private set; }
        public double Orbit { get; private set; }
        public double CentralDistance { get; private set; }
        public Color Color { get; private set; }
    }

    public sealed class SpotSample : ValueObject
    {
        public SpotSample(string title, double value, Color color, double centralDistance, double orbit)
        {
            this.Title = title;
            this.Value = value;
            Color = color;
            CentralDistance = centralDistance;
            Orbit = orbit;
        }

        public SpotSample(Color color, double centralDistance, double orbit)
        {
            Color = color;
            CentralDistance = centralDistance;
            Orbit = orbit;
        }

        public SpotSample()
        {
            //For ORM
        }

        public string Title { get; private set; }
        public double Orbit { get; private set; }
        public double CentralDistance { get; private set; }
        public double Value { get; private set; }
        public Color Color { get; private set; }
    }
}