using System;
using System.IO;

namespace LibProShip.Domain.Decode
{
    public sealed class RawReplay : Entity<RawReplay>
    {
        public Battle Battle { get; private set; }
        public byte[] Data { get; private set; }
        public string FileName { get; private set; }

        public RawReplay()
        {
        }

        public RawReplay(Guid id, String filename, Battle battle, byte[] data) : base(id)
        {
            this.Battle = battle;
            this.Data = data;
            this.FileName = filename;
        }
    }
}