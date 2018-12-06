namespace LibProShip.Application.VisualObject
{

    public class ReplayAbstract
    {
        public ReplayAbstract(string dateString, string ship)
        {
            this.DateString = dateString;
            this.Ship = ship;
        }

        public string DateString { get; }
        public string Ship { get; }
    }
}