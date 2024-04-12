namespace SportSync.Infrastructure.Storage.Settings;

public class BlobStorageSettings
{
    public const string SettingsKey = "BlobStorage";

    public string Url { get; set; }
    public string BucketFileLocation { get; set; }
    public string BucketName { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
}