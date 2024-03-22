using Amazon.S3;
using Amazon.S3.Transfer;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Infrastructure.Storage.Settings;

namespace SportSync.Infrastructure.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly ILogger<BlobStorageService> _logger;
    private readonly BlobStorageSettings _settings;

    public BlobStorageService(ILogger<BlobStorageService> logger, IOptions<BlobStorageSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<Result> UploadFile(string fileName, IFile file)
    {
        var s3ClientConfig = new AmazonS3Config
        {
            ServiceURL = _settings.Url
        };
        var s3Client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, s3ClientConfig);
        var fileTransferUtility = new TransferUtility(s3Client);

        try
        {
            await using Stream stream = file.OpenReadStream();
            await fileTransferUtility.UploadAsync(stream, _settings.BucketName, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to upload file {fileName}", fileName);
            return Result.Failure(DomainErrors.General.UnProcessableRequest);
        }

        return Result.Success();
    }
}