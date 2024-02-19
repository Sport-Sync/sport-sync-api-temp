using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class CreateUserTests : IntegrationTest
{
    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateSuccessfully()
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                mutation{
                    createUser(input: {firstName: ""Marko"", lastName: ""Zdravko"", email: ""email@gmail.com"",phone: ""+385916395254"", password: ""mypass1.23MM""}){
                        token
                    }
                }"));

        var tokenResponse = result.ToResponseObject<TokenResponse>("createUser");
        tokenResponse.Token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(tokenResponse.Token);

        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "userId").Value;

        var id = Guid.Parse(userId);

        var userFromDb = Database.DbContext.Set<User>().FirstOrDefault(x => x.Id == id);
        userFromDb.Should().NotBeNull();
        userFromDb.FirstName.Should().Be("Marko");
        userFromDb.LastName.Should().Be("Zdravko");
        userFromDb.Email.Should().Be("email@gmail.com");
        userFromDb.Phone.Value.Should().Be("0916395254");
    }

    [Fact]
    public async Task CreateUser_WithExistingEmail_ShouldFail()
    {
        Database.AddUser(email: "email@gmail.com");
        await Database.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                mutation{
                    createUser(input: {firstName: ""Marko"", lastName: ""Zdravko"", email: ""email@gmail.com"",phone: ""+385916395254"", password: ""mypass1.23MM""}){
                        token
                    }
                }"));

        result.ShouldHaveError(DomainErrors.User.DuplicateEmail.Message);
    }

    [Fact]
    public async Task CreateUser_WithExistingPhone_ShouldFail()
    {
        Database.AddUser(phone: "098376543");
        await Database.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                mutation{
                    createUser(input: {firstName: ""Marko"", lastName: ""Zdravko"", email: ""email@gmail.com"",phone: ""098376543"", password: ""mypass1.23MM""}){
                        token
                    }
                }"));

        result.ShouldHaveError(DomainErrors.User.DuplicatePhone.Message);
    }

    [Theory]
    [InlineData("+385987654321", "0987654321")]
    [InlineData("0983651322", "0983651322")]
    [InlineData("+38598 765243", "098765243")]
    [InlineData("+385 98 987 543", "098987543")]
    [InlineData("091 598 7643", "0915987643")]
    public async Task CreateUser_PhoneNumber_ShouldBeParsedToSingleFormat(string phoneNumberInput, string expectedPhoneSaved)
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation{{
                    createUser(input: {{firstName: ""Marko"", lastName: ""Zdravko"", email: ""email@gmail.com"",phone: ""{phoneNumberInput}"", password: ""mypass1.23MM""}}){{
                        token
                    }}
                }}"));

        var tokenResponse = result.ToResponseObject<TokenResponse>("createUser");

        tokenResponse.Token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(tokenResponse.Token);

        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "userId").Value;

        var id = Guid.Parse(userId);

        var userFromDb = Database.DbContext.Set<User>().FirstOrDefault(x => x.Id == id);
        userFromDb.Should().NotBeNull();
        userFromDb.Phone.Value.Should().Be(expectedPhoneSaved);

        Database.DbContext.Remove(userFromDb);
    }

    [Theory]
    [InlineData("passw", false, "The password is too short.")]
    [InlineData("PASSWORD", false, "The password requires at least one lowercase letter.")]
    [InlineData("password", false, "The password requires at least one uppercase letter.")]
    [InlineData("paSsword", false, "The password requires at least one digit.")]
    [InlineData("", false, "The password is required.")]
    [InlineData(null, false, "The password is required.")]
    [InlineData("paSsword1", true, null)]
    public async Task CreateUser_Password_ShouldMatchCriteria(string password, bool isValid, string error)
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation{{
                    createUser(input: {{firstName: ""Marko"", lastName: ""Zdravko"", email: ""email@gmail.com"",phone: ""0919279259"", password: ""{password}""}}){{
                        token
                    }}
                }}"));

        if (isValid)
        {
            var tokenResponse = result.ToResponseObject<TokenResponse>("createUser");
            tokenResponse.Token.Should().NotBeNullOrEmpty();
        }
        else
        {
            result.ShouldHaveError(error);
        }
    }

    [Theory]
    [InlineData("+38598765432", true)]
    [InlineData("+385987654321", true)]
    [InlineData("+38595765432", true)]
    [InlineData("+385957654326", true)]
    [InlineData("+38591765432", true)]
    [InlineData("+385917654323", true)]
    [InlineData("+38592765432", true)]
    [InlineData("+385927654432", true)]
    [InlineData("+38599765432", true)]
    [InlineData("+385997654432", true)]

    [InlineData("098765432", true)]
    [InlineData("0987654321", true)]
    [InlineData("095765432", true)]
    [InlineData("0957654326", true)]
    [InlineData("091765432", true)]
    [InlineData("0917654323", true)]
    [InlineData("092765432", true)]
    [InlineData("0927654432", true)]
    [InlineData("099765432", true)]
    [InlineData("0992765443", true)]

    [InlineData("+385 92 763 5432", true)]
    [InlineData("+385 92 763 532", true)]

    [InlineData("+385 9276 5432", false)]
    [InlineData("+38592 765 44 32", false)]
    [InlineData("0927 65 432", false)]
    [InlineData("091 7654 323", false)]
    [InlineData("091 765 3223", true)]

    [InlineData("+38593765432", false)]
    [InlineData("+38594765432", false)]
    [InlineData("+38596765432", false)]
    [InlineData("+38597765432", false)]

    [InlineData("093765432", false)]
    [InlineData("094765432", false)]
    [InlineData("096765432", false)]
    [InlineData("097765432", false)]

    [InlineData("0987654", false)]
    [InlineData("+38598765", false)]
    [InlineData("09812345678", false)]
    [InlineData("+359812345678", false)]
    public async Task CreateUser_PhoneNUmber_ShouldMatchCriteria(string phoneNumberInput, bool isValid)
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation{{
                    createUser(input: {{firstName: ""Marko"", lastName: ""Zdravko"", email: ""email@gmail.com"",phone: ""{phoneNumberInput}"", password: ""mypass1.23MM""}}){{
                        token
                    }}
                }}"));


        if (isValid)
        {
            var tokenResponse = result.ToResponseObject<TokenResponse>("createUser");
            tokenResponse.Token.Should().NotBeNullOrEmpty();
        }
        else
        {
            result.ShouldHaveError("Phone number is invalid.");
        }
    }
}