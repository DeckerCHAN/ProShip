using System.Threading.Tasks;

namespace LibProShipX.Command
{
    public interface ICommandRouter
    {
        Task ExecuteAsync(ICommand command);
    }
}