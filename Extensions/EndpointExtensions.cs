using System.Threading.Channels;
using AdIngestionAPI.Models;
using AdIngestionAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdIngestionAPI.Extensions;

public static class EndpointExtensions
{
    public static void MapIngestionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api").WithOpenApi();

        group.MapPost("/ingest", async ([FromBody] List<Lead> leads, Channel<Lead> channel, ILogger<Program> logger) =>
        {
            if (leads is null || leads.Count == 0)
                return Results.BadRequest("No leads provided.");

            // Write to channel
            foreach (var lead in leads)
            {
                await channel.Writer.WriteAsync(lead);
            }

            return Results.Accepted();
        })
        .WithName("IngestLeads");

        group.MapGet("/stats", (IngestionMetrics metrics) =>
        {
            return Results.Ok(metrics.GetSnapshot());
        })
        .WithName("GetStats");
    }
}
