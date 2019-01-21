namespace ProShipDesktop.ViewModels
{
    public class ClientUpgradeViewModel : ViewModelBase
    {
        private double Value1;

        public double Value
        {
            get => Value1;
            set
            {
                Value1 = value;
                this.OnPropertyChanged();
            }
        }
    }
}