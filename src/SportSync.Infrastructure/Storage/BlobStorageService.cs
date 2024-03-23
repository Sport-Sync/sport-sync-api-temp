using Amazon.S3;
using Amazon.S3.Model;
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

    public async Task<Result> UploadFile(string fileName, IFile file, CancellationToken cancellationToken)
    {
        try
        {
            var s3Client = GetS3Client();
            var fileTransferUtility = new TransferUtility(s3Client);
            await using Stream stream = file.OpenReadStream();
            await fileTransferUtility.UploadAsync(stream, _settings.BucketName, fileName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to upload file {fileName}", fileName);
            return Result.Failure(DomainErrors.General.UnProcessableRequest);
        }

        return Result.Success();
    }

    public async Task<string> GetDownloadUrl(string fileName)
    {
        try
        {
            var s3Client = GetS3Client();

            return await s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
            {
                BucketName = _settings.BucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddDays(1)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to generate download link for file {fileName}", fileName);
            return string.Empty;
        }
    }

    private AmazonS3Client GetS3Client()
    {
        var s3ClientConfig = new AmazonS3Config { ServiceURL = _settings.Url };
        var s3Client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, s3ClientConfig);
        return s3Client;
    }
}