using System.IO;

namespace LibProShip.Domain.Decode
{
    public sealed class RawReplay : IValueObject<RawReplay>
    {
        public RawReplay(Battle battle, Stream stream, string version)
        {
            this.Stream = stream;
            this.Version = version;
            this.Battle = battle;
        }

        public Stream Stream { get; }
        public Battle Battle { get; }

        public bool SameAs(RawReplay other)
        {
            throw new System.NotImplementedException();
        }

        public string Version { get; }
    }
}