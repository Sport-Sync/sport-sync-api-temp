using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class SetTerminAttendenceTests : IntegrationTest
{
    [Fact]
    public async Task CreateAttendence_ShouldFail_WhenTerminNotFound()
    {


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{Guid.NewGuid()}"", attending: false}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Termin.NotFound);
        //var response = result.ToObject<SetTerminAttendenceResponse>("setTerminAttendence");
    }

    [Fact]
    public async Task CreateAttendence_ShouldFail_WhenUserIsNotAPlayer()
    {
        var user = Database.AddUser();
        var termin = Database.AddUser();
        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{Guid.NewGuid()}"", attending: false}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Termin.PlayerNotFound);
        //var response = result.ToObject<SetTerminAttendenceResponse>("setTerminAttendence");
    }
}