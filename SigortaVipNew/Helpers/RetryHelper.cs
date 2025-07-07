using System;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVipNew.Helpers
{
    public static class RetryHelper
    {
        public static async Task<T> RetryAsync<T>(
            Func<Task<T>> operation,
            int maxRetries = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            var actualDelay = delay ?? TimeSpan.FromSeconds(1);
            Exception lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    ErrorLogger.LogError($"Deneme {attempt + 1}/{maxRetries + 1} başlatılıyor");
                    return await operation();
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    lastException = ex;
                    ErrorLogger.LogError(ex, $"Deneme {attempt + 1} başarısız, tekrar denenecek");

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await Task.Delay(actualDelay, cancellationToken);
                    actualDelay = TimeSpan.FromMilliseconds(actualDelay.TotalMilliseconds * 2); // Exponential backoff
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, $"Son deneme ({attempt + 1}) başarısız");
                    throw;
                }
            }

            ErrorLogger.LogError(lastException, "Tüm denemeler başarısız");
            throw lastException ?? new InvalidOperationException("Operation failed after retries");
        }

        public static async Task RetryAsync(
            Func<Task> operation,
            int maxRetries = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            await RetryAsync(async () =>
            {
                await operation();
                return true; // Dummy return value
            }, maxRetries, delay, cancellationToken);
        }

        public static T Retry<T>(
            Func<T> operation,
            int maxRetries = 3,
            TimeSpan? delay = null)
        {
            var actualDelay = delay ?? TimeSpan.FromSeconds(1);
            Exception lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    ErrorLogger.LogError($"Senkron deneme {attempt + 1}/{maxRetries + 1}");
                    return operation();
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    lastException = ex;
                    ErrorLogger.LogError(ex, $"Senkron deneme {attempt + 1} başarısız");
                    Thread.Sleep(actualDelay);
                    actualDelay = TimeSpan.FromMilliseconds(actualDelay.TotalMilliseconds * 2);
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, $"Senkron son deneme başarısız");
                    throw;
                }
            }

            throw lastException ?? new InvalidOperationException("Operation failed after retries");
        }
    }
}