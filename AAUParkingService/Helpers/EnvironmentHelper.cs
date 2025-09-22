using NCrontab;

namespace AAUParkingService.Helpers;

public static class EnvironmentHelper
{
    public static string GetEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Environment variable {name} is not set.");
        }

        return value;
    }

    public static string GetEnvironmentVariableCronExpression(string name)
    {
        var value = GetEnvironmentVariable(name);
        if (CrontabSchedule.TryParse(value) is null)
        {
            throw new InvalidOperationException($"Environment variable {name} '{value}' is not a valid cron expression.");
        }

        return value;
    }
}