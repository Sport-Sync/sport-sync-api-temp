using SportSync.Api.Tests.Common;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class AcceptTerminApplicationTests : IntegrationTest
{
    [Fact]
    public async Task AcceptTerminApplication_ShouldFail_WhenUserIsNotAnnouncer()
    {

    }

    [Fact]
    public async Task AcceptTerminApplication_ShouldSucceed_WhenUserIsAnnouncer()
    {

    }
}