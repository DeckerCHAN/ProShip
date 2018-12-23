namespace LibProShip.Domain.Analysis.Dto
{
    public sealed class SphereSample : ValueObject
    {
        public SphereSample(string title, int value)
        {
            this.Title = title;
            this.Value = value;
        }

        public SphereSample()
        {
            //For ORM
        }

        public string Title { get; private set; }
        public int Value { get; private set; }
    }
}