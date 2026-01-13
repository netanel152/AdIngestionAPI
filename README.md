# AdIngestionAPI

A high-performance .NET 9 Web API designed for rapid ad lead ingestion and real-time processing using asynchronous channels.

## Features

- **High-Throughput Ingestion**: Leverages `System.Threading.Channels` to decouple HTTP requests from processing logic, allowing for extremely high ingestion rates.
- **Asynchronous Background Processing**: A dedicated `BackgroundService` processes leads from the channel without blocking the API response.
- **Real-Time Metrics**: Thread-safe monitoring of total leads processed, average bid prices, and throughput (leads per second).
- **Modern .NET 9**: Built with the latest .NET performance optimizations and minimal APIs.

## Architecture

The API follows a producer-consumer pattern:
1. **Producer**: The `/api/ingest` endpoint receives batches of `Lead` objects and writes them into a bounded memory channel.
2. **Channel**: Acts as a high-performance, thread-safe buffer between the API and the background worker.
3. **Consumer**: `LeadProcessorService` (a `HostedService`) continuously reads from the channel and simulates lead processing.
4. **Metrics**: `IngestionMetrics` maintains a running state of processing statistics using thread-safe primitives.

## API Endpoints

### 1. Ingest Leads
- **URL**: `/api/ingest`
- **Method**: `POST`
- **Body**: Array of `Lead` objects.
- **Example Payload**:
  ```json
  [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "source": "GoogleAds",
      "bidPrice": 12.50,
      "region": "US-East",
      "riskScore": 0.05
    }
  ]
  ```
- **Response**: `202 Accepted`

### 2. Get Statistics
- **URL**: `/api/stats`
- **Method**: `GET`
- **Response**: `StatsSnapshot` object containing `totalLeadsProcessed`, `averageBidPrice`, and `leadsPerSecond`.

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Running the Application
1. Clone the repository.
2. Navigate to the project root.
3. Run the application:
   ```bash
   dotnet run
   ```
4. The API will be available at `http://localhost:5000` (or the port specified in `launchSettings.json`).
5. You can explore the API using the built-in OpenAPI documentation at `/openapi/v1.json` (or via Swagger if configured).

## Configuration
The channel capacity can be configured in `appsettings.json`:
```json
{
  "Ingestion": {
    "ChannelCapacity": 100000
  }
}
```
