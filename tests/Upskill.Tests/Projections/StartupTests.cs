using Projections;

namespace Upskill.Tests.Projections;

public class StartupTests
{
    [Fact]
    public void StartupTest()
    {
        //Arrange
        
        //Act
        var config = Startup.ConfigureAppConfiguration();
        var services = Startup.ConfigureServices(config);
        
        //Assert
        Assert.NotNull(config);
        Assert.NotNull(services);
    }
}