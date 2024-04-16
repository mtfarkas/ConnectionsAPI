namespace ConnectionsAPI.Config
{
    public class SyncOptions
    {
        /// <summary>
        /// The cron expression for the sync schedule
        /// </summary>
        public string? ScheduleCron { get; set; }
        /// <summary>
        /// If the sync should run immediately or wait until the next cron occurrence
        /// </summary>
        public bool? RunImmediately { get; set; }
    }
}
