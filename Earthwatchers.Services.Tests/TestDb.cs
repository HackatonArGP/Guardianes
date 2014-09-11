using Earthwatchers.Models;
using Moq;
using Xunit;

namespace Earthwatchers.Data
{
    public class DbTest
    {
        [Fact]
        public void GetEarthwatcherShouldReturnEarthwatcher()
        {
            // arrange
            var repos = new Mock<IEarthwatcherRepository>();
            var testEarthWatcher = new Earthwatcher() { Id = 3, Name = "Bert" };
            repos.Setup(soep => soep.GetEarthwatcher("bert", false)).Returns(testEarthWatcher);
            
            // act
            var result = repos.Object.GetEarthwatcher("bert", false);
            
            // assert
            Assert.Equal(3,result.Id);
        }
    }
}

