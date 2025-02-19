using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MinioBackgroundService : BackgroundService
{
    private readonly MinioService _minioService;
    private readonly string _bucketName;

    public MinioBackgroundService(MinioService minioService)
    {
        _minioService = minioService ?? throw new ArgumentNullException(nameof(minioService));
        _bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME") ?? "default-bucket";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Starting Minio bucket watcher for '{_bucketName}'...");

        try
        {

            using (var sub = await _minioService.WatchBucketAndNotifyAsync(_bucketName))
            {
                Console.WriteLine("Minio bucket watcher running. Press Ctrl+C to exit.");
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
    
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Minio watcher: {ex.Message}");
        }
    }
}