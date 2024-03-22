using HotChocolate.Types;
using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Users.UploadProfileImage;

public class UploadProfileImageInput : IRequest<Result>
{
    public IFile File { get; set; }
}