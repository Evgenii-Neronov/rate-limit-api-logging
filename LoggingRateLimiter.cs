using System.Collections.Concurrent;

public static class LoggingRateLimiter
{
    private static readonly ConcurrentDictionary<string, DateTime> LastLogTimeByType = new ConcurrentDictionary<string, DateTime>();

    /// <summary>
    /// Запросов в секунду
    /// </summary>
    private const int RequestLimitPerSecond = 10;

    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1) / RequestLimitPerSecond; 

    public static bool ShouldLog(string logType)
    {
        var now = DateTime.UtcNow;
        var lastLogAllowedTime = now - Interval;

        return LastLogTimeByType.AddOrUpdate(logType, now, (key, oldValue) =>
        {
            if (oldValue <= lastLogAllowedTime)
            {
                return now;
            }
            else
            {
                return oldValue;
            }
        }) == now;
    }
}