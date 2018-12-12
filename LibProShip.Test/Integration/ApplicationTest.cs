using System.Threading.Tasks;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class ApplicationTest
    {
        private Application.Application Application;
        public ApplicationTest()
        {
            this.Application = new Application.Application();
        }

        [Fact]
        public async Task Test1()
        {
            await Task.Delay(90000);

        }
        
    }
}