using HotChocolate.Types;

namespace SportSync.Application.Core.Abstractions.Storage;

public interface IBlobStorageService
{
    Task UploadFile(string fileName, IFile file);
}