using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Users.GetByPhoneNumbers;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class GetUsersByPhoneNumbersTests : IntegrationTest
{
    [Fact]
    public async Task GetByPhoneNumbers_ShouldReturn_WhenFound()
    {
        var currentUser = Database.AddUser();
        var user1 = Database.AddUser(phone: "0919279259");
        var user2 = Database.AddUser(phone: "099927 9259");
        var user3 = Database.AddUser(phone: "+385 99927 3333");
        var user4 = Database.AddUser(phone: "0929279259");

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                query{
                    usersByPhoneNumbers(input: {phoneNumbers: [""0959279259"", ""0919279259"", ""099927 9259"", ""+385 99927 3333""]}){
                        users{
                            firstName, email, phone, id
                        }
                    }
                }"));

        var userResponse = result.ToResponseObject<GetUsersByPhoneNumbersResponse>("usersByPhoneNumbers");

        userResponse.Users.Count.Should().Be(3);
        userResponse.Users.FirstOrDefault(x => x.Id == user1.Id).Should().NotBeNull();
        userResponse.Users.FirstOrDefault(x => x.Id == user2.Id).Should().NotBeNull();
        userResponse.Users.FirstOrDefault(x => x.Id == user3.Id).Should().NotBeNull();
    }

    [Theory]
    [InlineData("0998026836", "0998026836")]
    [InlineData("+385998026836", "0998026836")]
    [InlineData("+385 9980 26836", "0998026836")]
    [InlineData("(+385) 9980 26836", "0998026836")]
    [InlineData("099-8026-836", "0998026836")]
    [InlineData("099/8026/836", "0998026836")]
    public async Task GetByPhoneNumbers_ShouldSanitize_ThenReturnExistingUsers(string inputPhoneNumber, string existingUserPhoneNumber)
    {
        var currentUser = Database.AddUser();
        var user1 = Database.AddUser(phone: existingUserPhoneNumber);

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    usersByPhoneNumbers(input: {{phoneNumbers: [""{inputPhoneNumber}""]}}){{
                        users{{
                            firstName, email, phone, id
                        }}
                    }}
                }}"));

        var userResponse = result.ToResponseObject<GetUsersByPhoneNumbersResponse>("usersByPhoneNumbers");

        userResponse.Users.Count.Should().Be(1);
        userResponse.Users.FirstOrDefault(x => x.Id == user1.Id).Should().NotBeNull();
    }
}