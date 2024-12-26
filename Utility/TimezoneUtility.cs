namespace ConnectionsAPI.Utility;
public static class TimezoneUtility
{
    public static TimeZoneInfo? GetLatestTimezoneOnSystem() =>
        TimeZoneInfo.GetSystemTimeZones()
            .OrderByDescending(x => x.GetUtcOffset(DateTime.UtcNow))
            .FirstOrDefault();
}
