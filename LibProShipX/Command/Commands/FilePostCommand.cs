namespace LibProShipX.Command.Commands
{
    public sealed class FilePostCommand: ICommand
    {
        public byte [] FileContent { get; }
    }
}