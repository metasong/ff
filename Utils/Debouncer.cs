namespace ff.Utils;

/// <summary>
/// Delays invoking an action until a certain amount of time has passed 
/// without another call, effectively executing only the last call in a series.
/// </summary>
public class Debouncer
{
    private readonly TimeSpan interval;
    private CancellationTokenSource? cancellationTokenSource;
//#define TEST_DEBOUNCER
#if TEST_DEBOUNCER
    private long _lastFireTimestamp;
#endif
    /// <summary>
    /// Initializes a new instance of the Debouncer class.
    /// </summary>
    /// <param name="interval">The quiet period required before the action is invoked.</param>
    public Debouncer(TimeSpan interval)
    {
        this.interval = interval;
    }
    public Debouncer(double intervalMilliseconds)
    {
        this.interval = TimeSpan.FromMilliseconds(intervalMilliseconds);
    }
    /// <summary>
    /// Schedules the action to be executed after the interval. If called again before
    /// the interval expires, the previous pending action is cancelled and the timer is reset.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Debounce(Action action, ILogger? logger = null)
    {
        // Cancel any previously scheduled action
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        long eventTimestamp = Stopwatch.GetTimestamp();
        // Schedule the new action to run after the interval
        Task.Delay(interval, cancellationTokenSource.Token)
            .ContinueWith(task =>
            {
                // Only run the action if the task completed successfully (was not cancelled)
                if (task.IsCompletedSuccessfully)
                {
#if TEST_DEBOUNCER
                    long currentTimestamp = Stopwatch.GetTimestamp();
                    double elapsedTicks = currentTimestamp - eventTimestamp;
                    double lastFire = currentTimestamp - _lastFireTimestamp;

                    _lastFireTimestamp = currentTimestamp;
                    logger.LogInformation($"debounce fire: eventTicks: {elapsedTicks/Stopwatch.Frequency * 1000}ms, lastFire: {lastFire / Stopwatch.Frequency * 1000}ms ");
#endif
                    action();
                }
#if TEST_DEBOUNCER
                else
                {
                    long currentTimestamp = Stopwatch.GetTimestamp();
                    double elapsedTicks = currentTimestamp - eventTimestamp;
                    double lastFire = currentTimestamp - _lastFireTimestamp;

                    logger.LogInformation($"debounce cancel: elapsedTicks: {elapsedTicks/Stopwatch.Frequency * 1000}ms,  lastFire: {lastFire / Stopwatch.Frequency * 1000}ms");
                }
#endif
            }, TaskScheduler.Default); // Use a default scheduler
    }
}