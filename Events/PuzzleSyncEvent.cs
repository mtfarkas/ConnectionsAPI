
using ConnectionsAPI.Database;
using ConnectionsAPI.Utility;
using System.Diagnostics;

namespace ConnectionsAPI.Events
{
    public class PuzzleSyncEvent : IEvent { }

    public class PuzzleSyncHandler(ILogger<PuzzleSyncHandler> logger, IServiceScopeFactory scopeFactory) : IEventHandler<PuzzleSyncEvent>
    {
        private readonly ILogger<PuzzleSyncHandler> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task HandleAsync(PuzzleSyncEvent eventModel, CancellationToken ct)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Received Puzzle Sync Event");
            try
            {
                // construct scope 
                using var scope = _scopeFactory.CreateScope();
                // get dependencies
                ConnectionsContext db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
                HttpClient http = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
                ILogger<SyncUtility> syncLogger = scope.ServiceProvider.GetRequiredService<ILogger<SyncUtility>>();
                // do the work
                await new SyncUtility(db, syncLogger, http).SyncPuzzlesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing puzzle sync event");
            }
            finally
            {
                stopwatch.Stop();
            }
            _logger.LogInformation("Puzzle Sync Event finished in {ts}", stopwatch.Elapsed);
        }
    }
}
