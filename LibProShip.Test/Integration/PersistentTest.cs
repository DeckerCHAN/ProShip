using System;
using System.Linq;
using System.Threading.Tasks;
using LibProShip.Domain;
using LibProShip.Infrastructure.Repo;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class TestValueObject : ValueObject<TestValueObject>
    {
        public TestValueObject()
        {
            this.value = null;
        }
        
        public TestValueObject(string value)
        {
            this.value = value;
        }

        public string value { get; private set; }
    }
    
    public class TestEntity : Entity<TestEntity>
    {
        public string Name { get; private set; }
        public TestValueObject TestValueObject { get;private set; }
        
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
            Assert.Equal(firstOrDefault,item);
        }
    }
}