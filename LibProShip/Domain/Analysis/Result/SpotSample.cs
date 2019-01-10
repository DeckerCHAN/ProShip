namespace LibProShip.Domain.Analysis.Result
{
    public sealed class PointSample : ValueObject
    {
        public string Title { get; private set; }
    }

    public sealed class SpotSample : ValueObject
    {
        public SpotSample(string title, int value)
        {
            this.Title = title;
            this.Value = value;
        }

        public SpotSample()
        {
            //For ORM
        }

        public string Title { get; private set; }
        public int Value { get; private set; }
        
    }
}