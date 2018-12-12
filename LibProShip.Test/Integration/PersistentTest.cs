using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain;
using LibProShip.Domain.Decode;
using LibProShip.Infrastructure.Repo;
using LiteDB;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class TestValueObject : ValueObject<TestValueObject>
    {
        public TestValueObject()
        {
            this.value = string.Empty;
        }

        public TestValueObject(string value)
        {
            this.value = value;
        }

        public string value { get; private set; }
    }

    public class TestEntity : Entity<TestEntity>
    {
        public string Name { get; set; }
        public TestValueObject TestValueObject { get; set; }

        public TestEntity()
        {
        }

        public TestEntity(Guid id, string name) : base(id)
        {
            this.TestValueObject = new TestValueObject("ThisIsVO");
            this.Name = name;
        }
    }

    public class PersistentTest
    {
        public PersistentTest()
        {
            this.EntityRepo = new Repository<TestEntity>();
        }

        public Repository<TestEntity> EntityRepo { get; set; }

        [Fact]
        public async Task TestSave()
        {
            var item = new TestEntity(Guid.NewGuid(), "FirstItem");
            this.EntityRepo.Insert(item);
            var firstOrDefault = this.EntityRepo.GetAll().FirstOrDefault(x => Equals(x, item));
            Assert.Equal(firstOrDefault, item);
        }

        [Fact]
        public async Task TestSaveReplay()
        {
            var vehicles = new List<Vehicle>();
            var player = new Player(1, "TestUser");
            vehicles.Add(new Vehicle(1, 1, 1, player));

            var battle = new Battle("1.7.10", 1200, player, vehicles);
            var rawReplay = new RawReplay(Guid.NewGuid(), "F:/Filename.wowsreplay", battle,
                new byte[] {123, 45, 2, 34, 56, 77, 12});
            
            var repo = new RawReplayRepo();
            repo.Insert(rawReplay);
        }
    }

    public class RawReplay : Entity<RawReplay>
    {
        public Battle Battle { get; private set; }
        public byte[] Bytes { get; private set; }
        public string FileName { get; private set; }

        public RawReplay()
        {
        }

        public RawReplay(Guid id, String filename, Battle battle, byte[] bytes) : base(id)
        {
            this.Battle = battle;
            this.Bytes = bytes;
            this.FileName = filename;
        }
    }

    public sealed class RawReplayRepo : IRepository<RawReplay>
    {
        private LiteStorage FileStorage { get; set; }

        public RawReplayRepo()
        {
            var fileName = $"RawReplay.ldb";
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }

            this.Context = new LiteDatabase(fileName);
            this.Collection = this.Context.GetCollection<RawReplay>();
            this.FileStorage = this.Context.FileStorage;
        }

        private LiteCollection<RawReplay> Collection { get; set; }

        private LiteDatabase Context { get; set; }

        public void Insert(RawReplay item)
        {
            var bytes = item.Bytes;
            var simpReplay = new RawReplay(item.Id, item.FileName, item.Battle, null);
            this.Collection.Insert(simpReplay);
            using (var zipedStream = this.Compress(bytes))
            using (var stream = this.FileStorage.OpenWrite(simpReplay.Id.ToString(), simpReplay.FileName))
            {
                zipedStream.CopyTo(stream);
            }
        }

        public void Update(RawReplay item)
        {
            throw new NotImplementedException();
        }

        public void Remove(RawReplay item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RawReplay> GetAll()
        {
            throw new NotImplementedException();
        }


        private byte[] Decompress(Stream gzipData)
        {
//            var output = new MemoryStream();
//            using (var compressedStream = new MemoryStream(new ArraySegment<byte>(data, 2, data.Length - 2).ToArray()))
//            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
//            {
//                zipStream.CopyTo(output);
//                zipStream.Close();
//                output.Position = 0;
//                return output;
//            }
            throw new NotImplementedException();
        }

        private Stream Compress(byte[] data)
        {
            var output = new MemoryStream();

            using (var originStream = new MemoryStream(data))
            using (var def = new DeflateStream(output, CompressionLevel.Optimal))
            {
                originStream.CopyTo(def);
                return output;
            }
        }
    }
}