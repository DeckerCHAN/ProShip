namespace LibProShip.Application.VisualObject
{
    public class ReplayAbstract
    {
        public ReplayAbstract(string dateString, string ship, string id)
        {
            DateString = dateString;
            Ship = ship;
            this.id = id;
        }

        public string id { get; }
        public string DateString { get; }
        public string Ship { get; }
    }
}