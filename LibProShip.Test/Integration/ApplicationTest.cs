using System.Threading.Tasks;
using Xunit;

namespace LibProShip.Test.Integration
{
    public class ApplicationTest
    {
        public ApplicationTest()
        {
            Application = new Application.Application();
        }

        private Application.Application Application;

        [Fact]
        public async Task Run90Seconds()
        {
            await Task.Delay(90000);
        }
    }
}