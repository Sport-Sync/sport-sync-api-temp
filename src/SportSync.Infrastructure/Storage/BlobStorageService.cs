using Amazon.S3;
using Amazon.S3.Transfer;
using HotChocolate.Types;
using SportSync.Application.Core.Abstractions.Storage;

namespace SportSync.Infrastructure.Storage;

public class BlobStorageService : IBlobStorageService
{
    public async Task UploadFile(string fileName, IFile file)
    {
        var s3ClientConfig = new AmazonS3Config
        {
            ServiceURL = "https://fra1.digitaloceanspaces.com"
        };
        var s3Client = new AmazonS3Client("DO00Z9QXK44N2639E3RE", "c04bDBUXM5+OaTLRZLV3EwG7nv9zO/vKLNUjBbYCv7c", s3ClientConfig);
        var fileTransferUtility = new TransferUtility(s3Client);

        await using Stream stream = file.OpenReadStream();

        try
        {
            var extension = file.Name.Split(".").Last();
            await fileTransferUtility.UploadAsync(stream, "sport-sync", "profile-images/" + fileName + "." + extension);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}