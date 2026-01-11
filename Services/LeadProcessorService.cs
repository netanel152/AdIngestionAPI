using System.Threading.Channels;
using AdIngestionAPI.Models;

namespace AdIngestionAPI.Services;

public class LeadProcessorService : BackgroundService
{
    private readonly Channel<Lead> _channel;
    private readonly IngestionMetrics _metrics;
    private readonly ILogger<LeadProcessorService> _logger;

    public LeadProcessorService(Channel<Lead> channel, IngestionMetrics metrics, ILogger<LeadProcessorService> logger)
    {
        _channel = channel;
        _metrics = metrics;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Lead Processor Service Started.");

        // ReadAllAsync is efficient.
        // For higher performance in real-world, we might use Batching (e.g. read 100 items or wait 100ms)
        // to reduce lock contention on the metrics or DB IO.
        // Given the requirement to "simulate processing" individually, we keep the loop simple but clean.
        
        try 
        {
            await foreach (var lead in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                await ProcessLeadAsync(lead, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Lead Processor Service encountered a fatal error.");
        }
        finally
        {
            _logger.LogInformation("Lead Processor Service Stopped.");
        }
    }

    private async Task ProcessLeadAsync(Lead lead, CancellationToken ct)
    {
        try
        {
            // Simulate processing logic
            // Using Task.Delay(1) to ensure we don't just spin-wait and consume 100% CPU on empty loop logic if this were real work.
            await Task.Delay(1, ct); 

            _metrics.RecordLead(lead.BidPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing lead {LeadId}", lead.Id);
        }
    }
}
