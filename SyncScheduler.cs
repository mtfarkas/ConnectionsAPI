using ConnectionsAPI.Events;
using Cronos;
using System.Diagnostics.CodeAnalysis;

namespace ConnectionsAPI
{
    public class SyncScheduler(ILogger<SyncScheduler> logger, IConfiguration configuration) : BackgroundService
    {
        private readonly ILogger<SyncScheduler> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // get and parse the cron expression
            string configCronStr = _configuration.GetValue<string>("Sync:ScheduleCron") ?? string.Empty;
            if (!TryParseCronExpression(configCronStr, out var cron))
            {
                cron = CronExpression.Hourly;
                _logger.LogWarning("Passed CRON expression was invalid ({configCron}); Defaulting to {newCron}", configCronStr, cron.ToString());
            }
            _logger.LogInformation("Starting Sync Scheduler with CRON expression {cron}", cron.ToString());

            // get and parse if immediate execution is wanted
            bool runImmediately = _configuration.GetValue<bool>("Sync:RunImmediately", false);
            if (runImmediately)
            {
                _logger.LogInformation("Immediate execution enabled; Sending sync command.");
                await SendSyncEvent(stoppingToken, wait: true);
            }

            // run the loop for executions
            while (!stoppingToken.IsCancellationRequested)
            {
                // wait for next scheduled run
                await WaitForNextSchedule(cron, stoppingToken);
                // run the sync
                _logger.LogInformation("Sending sync command at: {currentTime}", DateTimeOffset.UtcNow);
                await SendSyncEvent(stoppingToken, wait: false);
            }
        }

        // this method exists because the Cronos TryParse for some reason throws an exception on nulls and empty strings
        // completely defeating the purpose of the TryParse pattern
        private static bool TryParseCronExpression(string expression, [NotNullWhen(true)] out CronExpression? cron)
        {
            try
            {
                cron = CronExpression.Parse(expression);
                return true;
            }
            catch
            {
                cron = null;
                return false;
            }
        }

        private static Task SendSyncEvent(CancellationToken stoppingToken, bool wait = false) =>
            new ConnectionsSyncEvent { }.PublishAsync(wait ? Mode.WaitForAll : Mode.WaitForNone, stoppingToken);

        private async Task WaitForNextSchedule(CronExpression cron, CancellationToken ct)
        {
            var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
            var nextOccurrenceTime = cron.GetNextOccurrence(currentUtcTime);

            var delay = nextOccurrenceTime.GetValueOrDefault() - currentUtcTime;

            _logger.LogInformation("Run delayed for {delay}. Next occurrence: {nextOccurrence}; Current time: {currentTime}", delay, nextOccurrenceTime, currentUtcTime);

            await Task.Delay(delay, ct);
        }
    }
}
