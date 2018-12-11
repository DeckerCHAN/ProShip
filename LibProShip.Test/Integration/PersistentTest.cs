using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain;
using LibProShip.Domain.Decode;
using LibProShip.Infrastructure.Repo;
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
//            var vehicles = new List<Vehicle>();
//            var player = new Player(1, "TestUser");
//            vehicles.Add(new Vehicle(1, 1, 1, player));
//
//            var battle = new Battle("1.7.10", 1200, player, vehicles);
//            var packets = new List<Packet>();
//            var packet = new Packet(1,1,);
//            var replay = new Replay(Guid.NewGuid(), battle,DateTime.Now,);
        }
    }
}