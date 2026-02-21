using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 提供带有重试逻辑的操作执行工具方法。
    /// </summary>
    public static class RetryUtility
    {
        private struct VoidReturn
        {
        }

        public static UniTask RunAsync(
            [NotNull] Func<UniTask> actionExecutor,
            [NotNull] IRetryPolicy policy,
            int maxRetryCount = 3)
        {
            if (actionExecutor == null)
                throw new ArgumentNullException(nameof(actionExecutor));
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));
            if (maxRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetryCount), "Max retry count cannot be negative.");

            return RunInternalAsync(async () =>
            {
                await actionExecutor();
                return new VoidReturn();
            }, policy, maxRetryCount, 0, new List<Exception>());
        }

        public static UniTask<T> RunAsync<T>(
            [NotNull] Func<UniTask<T>> actionExecutor,
            [NotNull] IRetryPolicy policy,
            int maxRetryCount = 3)
        {
            if (actionExecutor == null)
                throw new ArgumentNullException(nameof(actionExecutor));
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));
            if (maxRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetryCount), "Max retry count cannot be negative.");

            return RunInternalAsync(actionExecutor, policy, maxRetryCount, 0, new List<Exception>());
        }

        private static async UniTask<T> RunInternalAsync<T>(
            Func<UniTask<T>> actionExecutor,
            IRetryPolicy policy,
            int maxRetryCount,
            int retryCount,
            List<Exception> exceptions)
        {
            try
            {
                return await actionExecutor();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
                if (retryCount >= maxRetryCount)
                {
                    throw new MaxRetryCountExceededException(maxRetryCount, exceptions);
                }

                bool shouldRetry = await policy.ShouldRetryAsync(e);
                if (!shouldRetry)
                {
                    throw new AggregateException($"Operation failed after {retryCount} retry attempts.", exceptions);
                }

                return await RunInternalAsync(actionExecutor, policy, maxRetryCount, retryCount + 1, exceptions);
            }
        }
    }
}
