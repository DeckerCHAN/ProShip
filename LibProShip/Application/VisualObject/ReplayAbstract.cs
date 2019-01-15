namespace LibProShip.Application.VisualObject
{
    public class ReplayAbstract
    {
        public ReplayAbstract(string dateString, string ship, string id)
        {
            this.DateString = dateString;
            this.Ship = ship;
            this.id = id;
        }

        public string id { get; }
        public string DateString { get; }
        public string Ship { get; }
    }
}