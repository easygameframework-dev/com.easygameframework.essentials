using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 提供带有重试逻辑的操作执行工具方法。
    /// </summary>
    public static class RetryUtility
    {
        /// <summary>
        /// 根据指定策略执行带有重试逻辑的异步操作。
        /// </summary>
        /// <param name="actionExecutor">操作执行器，接收失败回调作为参数。</param>
        /// <param name="policy">应用的重试策略。</param>
        /// <param name="maxRetryCount">最大重试次数。</param>
        /// <param name="onFailure">当超过重试次数或者重试过程中抛出异常时调用的回调。</param>
        public static void Run(
            [NotNull] Action<Action<Exception>> actionExecutor,
            [NotNull] IRetryPolicy policy,
            [NotNull] Action<Exception> onFailure,
            int maxRetryCount = 3)
        {
            if (actionExecutor == null)
                throw new ArgumentNullException(nameof(actionExecutor));
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));
            if (onFailure == null)
                throw new ArgumentNullException(nameof(onFailure));
            if (maxRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetryCount), "Max retry count cannot be negative.");

            RunInternal(actionExecutor, policy, maxRetryCount, 0, new List<Exception>(), onFailure);
        }

        private static void RunInternal(
            Action<Action<Exception>> actionExecutor,
            IRetryPolicy policy,
            int maxRetryCount,
            int retryCount,
            List<Exception> exceptions,
            Action<Exception> onFailure)
        {
            actionExecutor(OnFailed);
            return;

            void OnFailed(Exception exception)
            {
                exceptions.Add(exception);

                policy.ShouldRetry(
                    retryCount,
                    exception,
                    onDecision: OnDecision,
                    onRetryPolicyFailed: OnRetryPolicyFailed);
                return;

                void OnDecision(bool shouldRetry)
                {
                    if (shouldRetry && retryCount < maxRetryCount)
                    {
                        RunInternal(actionExecutor, policy, maxRetryCount, retryCount + 1, exceptions, onFailure);
                    }
                    else
                    {
                        Exception failedException = retryCount >= maxRetryCount
                            ? new MaxRetryCountExceededException(maxRetryCount, exceptions)
                            : new AggregateException($"Operation failed after {retryCount} retry attempts.", exceptions);

                        onFailure(failedException);
                    }
                }

                void OnRetryPolicyFailed(Exception policyException)
                {
                    var aggregateException = new AggregateException("Retry policy execution failed.", exception, policyException);
                    onFailure(aggregateException);
                }
            }
        }
    }
}
