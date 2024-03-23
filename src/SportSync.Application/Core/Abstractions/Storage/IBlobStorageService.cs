using HotChocolate.Types;
using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Core.Abstractions.Storage;

public interface IBlobStorageService
{
    Task<Result> UploadFile(string fileName, IFile file, CancellationToken cancellationToken);
    Task<Result> RemoveFile(string fileName, CancellationToken cancellationToken);
    Task<string> GetDownloadUrl(string fileName);
}