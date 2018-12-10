using System;
using System.Threading.Tasks;
using LibProShip.Infrastructure.Repo;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class TestEntity
    {
        public TestEntity(string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public Guid Id { get; }
        public string Name { get; }
    }

    public class PersistentTest
    {
        public PersistentTest()
        {
            this.EntityRepo = new RepositoryBase<TestEntity>();
        }

        public RepositoryBase<TestEntity> EntityRepo { get; set; }

        [Fact]
        public async Task TestSave()
        {
            EntityRepo.Insert(new );
        }
    }
}