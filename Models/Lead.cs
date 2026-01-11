namespace AdIngestionAPI.Models;

public record Lead(Guid Id, string Source, decimal BidPrice, string Region, double RiskScore);
