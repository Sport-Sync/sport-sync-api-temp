using HotChocolate;

namespace SportSync.Domain.Types.Abstraction;

public interface IProfileImageUser
{
    Guid Id { get; set; }
    string ImageUrl { get; set; }

    [GraphQLIgnore]
    bool HasProfileImage { get; set; }
}