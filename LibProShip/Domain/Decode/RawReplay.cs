using System.IO;

namespace LibProShip.Domain.Decode
{
    public sealed class RawReplay : ValueObject<RawReplay>
    {
        public RawReplay(Battle battle, Stream stream)
        {
            this.Stream = stream;
            this.Battle = battle;
        }

        private Stream BackedStream;

        public Stream Stream
        {
            get
            {
                var st = new MemoryStream();
                this.BackedStream.CopyTo(st);
                st.Seek(0, SeekOrigin.Begin);
                this.BackedStream.Seek(0, SeekOrigin.Begin);
                return st;
            }


            private set => this.BackedStream = value;
        }

        public Battle Battle { get; }

        public bool SameAs(RawReplay other)
        {
            throw new System.NotImplementedException();
        }

    }
}