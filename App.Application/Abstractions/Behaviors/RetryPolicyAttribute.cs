namespace App.Application.Abstractions.Behaviors;

[AttributeUsage(AttributeTargets.Class)]
public class RetryPolicyAttribute : Attribute
{
    public int RetryCount { get; }
    public int SleepDurationMilliseconds { get; }

    /// <summary>
    /// Bu Attribute'u kullandığın Command/Query hata alırsa tekrar denenir.
    /// </summary>
    /// <param name="retryCount">Kaç kere denensin? (Default: 3)</param>
    /// <param name="sleepDurationMilliseconds">Her deneme arası kaç ms beklesin? (Default: 200ms)</param>
    public RetryPolicyAttribute(int retryCount = 3, int sleepDurationMilliseconds = 200)
    {
        RetryCount = retryCount;
        SleepDurationMilliseconds = sleepDurationMilliseconds;
    }
}