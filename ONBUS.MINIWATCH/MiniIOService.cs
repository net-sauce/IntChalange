using Minio;
using Minio.DataModel;
using MassTransit;
using System.Threading.Tasks;
using Minio.DataModel.Args;
using Minio.DataModel.Notification;
using System.Reactive.Linq;
using System.Reactive;
using System.Text.Json;
using COMMON.CONTRACTS.File;


public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public MinioService(IPublishEndpoint publishEndpoint)
    {
        Console.WriteLine("Initializing Minio client...");

        _minioClient = new MinioClient()
            .WithEndpoint(Environment.GetEnvironmentVariable("MINIO_ENDPOINT") ?? "localhost:9000")
            .WithCredentials(
                Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY") ?? "minio123",
                Environment.GetEnvironmentVariable("MINIO_SECRET_KEY") ?? "minio123"
            )
            .Build();

        Console.WriteLine("Minio client initialized");

        _publishEndpoint = publishEndpoint;
        Console.WriteLine("Publish endpoint initialized");
    }

    public async Task<IDisposable> WatchBucketAndNotifyAsync(string bucketName)
    {
        Console.WriteLine($"Checking if bucket '{bucketName}' exists...");

        bool found = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucketName));

        if (!found)
        {
            Console.WriteLine($"Bucket '{bucketName}' does not exist. Creating the bucket...");
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucketName));
            Console.WriteLine($"Bucket '{bucketName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Bucket '{bucketName}' already exists. Proceeding...");
        }

        Console.WriteLine($"Watching bucket: {bucketName}");

        var observable = _minioClient.ListenBucketNotificationsAsync(
            new ListenBucketNotificationsArgs().WithPrefix("").WithSuffix("")
                .WithBucket(bucketName)
                .WithEvents(new List<EventType>() { EventType.ObjectCreatedAll })
        );

        return observable.Subscribe(
            notification =>
            {
                Console.WriteLine("Received a bucket notification.");

                try
                {
                    var notificationRecord = JsonSerializer.Deserialize<RecordsModel>(notification.Json);
                    Console.WriteLine($"Deserialized notification record: {notification.Json}");
                    foreach (var record in notificationRecord.Records)
                    {


                        var parts = record.S3.Object.Key.Split("/");
                        if (parts != null && parts.Length > 2)
                        {
                            Console.WriteLine($"File uploaded: JobID={parts[1]}, FileName={parts[2]}");

                            // Publish an event with the file upload details
                            _publishEndpoint.Publish(new OnFileUploaded
                            {
                                JobID = Guid.Parse(parts[1]),
                                FileName = parts[2],
                                ClientID = parts[0]
                            });

                            Console.WriteLine($"Published event for JobID={parts[1]}, FileName={parts[2]}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing bucket notification: {ex.Message}");
                }
            },
            ex =>
            {
                Console.WriteLine($"Error in bucket notification subscription: {ex}");
            },
            () =>
            {
                Console.WriteLine("Stopped listening for bucket notifications");
            });


    }
}