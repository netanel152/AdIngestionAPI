namespace AdIngestionAPI.Models;

public record StatsSnapshot(long TotalLeadsProcessed, decimal AverageBidPrice, double LeadsPerSecond);
