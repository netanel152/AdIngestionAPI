using System.Threading.Channels;
using AdIngestionAPI.Models;
using AdIngestionAPI.Services;

namespace AdIngestionAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIngestionServices(this IServiceCollection services, IConfiguration config)
    {
        // Thread-safe statistics container
        services.AddSingleton<IngestionMetrics>();

        // High-throughput Channel
        // Configuration could be pulled from appsettings.json
        var channelCapacity = config.GetValue<int>("Ingestion:ChannelCapacity", 100_000);
        
        services.AddSingleton<Channel<Lead>>(_ => 
            Channel.CreateBounded<Lead>(new BoundedChannelOptions(channelCapacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            }));

        // Register the Background Worker
        services.AddHostedService<LeadProcessorService>();

        return services;
    }
}
