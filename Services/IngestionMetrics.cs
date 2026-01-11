using AdIngestionAPI.Models;

namespace AdIngestionAPI.Services;

public class IngestionMetrics
{
    private long _totalLeads;
    private decimal _totalBidSum; 
    private readonly object _lock = new();
    private readonly DateTime _startTime = DateTime.UtcNow;

    public void RecordLead(decimal bidPrice)
    {
        lock (_lock)
        {
            _totalLeads++;
            _totalBidSum += bidPrice;
        }
    }

    public StatsSnapshot GetSnapshot()
    {
        lock (_lock)
        {
            var elapsedSeconds = (DateTime.UtcNow - _startTime).TotalSeconds;
            // Prevent divide by zero
            var avgBid = _totalLeads > 0 ? _totalBidSum / _totalLeads : 0;
            var throughput = elapsedSeconds > 0.001 ? _totalLeads / elapsedSeconds : 0;

            return new StatsSnapshot(
                _totalLeads, 
                Math.Round(avgBid, 2), 
                Math.Round(throughput, 2)
            );
        }
    }
}
