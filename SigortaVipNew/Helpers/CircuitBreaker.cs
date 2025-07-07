using System;
using System.Threading.Tasks;

namespace SigortaVipNew.Helpers
{
    public class CircuitBreaker
    {
        private readonly int _failureThreshold;
        private readonly TimeSpan _timeout;
        private int _failureCount;
        private DateTime _lastFailureTime;
        private CircuitState _state;

        public CircuitBreaker(int failureThreshold = 5, TimeSpan? timeout = null)
        {
            _failureThreshold = failureThreshold;
            _timeout = timeout ?? TimeSpan.FromMinutes(1);
            _state = CircuitState.Closed;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.Now - _lastFailureTime >= _timeout)
                {
                    _state = CircuitState.HalfOpen;
                    ErrorLogger.LogError("Circuit breaker half-open durumuna geçti");
                }
                else
                {
                    var remainingTime = _timeout - (DateTime.Now - _lastFailureTime);
                    throw new InvalidOperationException($"Circuit breaker açık. Kalan süre: {remainingTime.TotalSeconds:F0} saniye");
                }
            }

            try
            {
                var result = await operation();

                if (_state == CircuitState.HalfOpen)
                {
                    _state = CircuitState.Closed;
                    _failureCount = 0;
                    ErrorLogger.LogError("Circuit breaker kapalı duruma döndü");
                }

                return result;
            }
            catch (Exception ex)
            {
                _failureCount++;
                _lastFailureTime = DateTime.Now;

                if (_failureCount >= _failureThreshold)
                {
                    _state = CircuitState.Open;
                    ErrorLogger.LogError($"Circuit breaker açıldı. Hata sayısı: {_failureCount}");
                }

                ErrorLogger.LogError(ex, $"Circuit breaker hatası. Toplam hata: {_failureCount}");
                throw;
            }
        }

        public T Execute<T>(Func<T> operation)
        {
            return ExecuteAsync(() => Task.FromResult(operation())).Result;
        }

        public CircuitState State => _state;
        public int FailureCount => _failureCount;

        public enum CircuitState
        {
            Closed,   // Normal çalışma
            Open,     // Hata çok fazla, bloklanmış
            HalfOpen  // Test durumu
        }
    }
}