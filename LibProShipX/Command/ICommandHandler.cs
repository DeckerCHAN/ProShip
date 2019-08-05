namespace LibProShipX.Command
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        ICommandResult Handle(T command);
    }
}