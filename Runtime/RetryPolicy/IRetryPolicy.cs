using System;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 定义失败操作的重试策略。
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// 确定操作失败后是否应该重试。
        /// </summary>
        /// <param name="retryCount">已进行的重试次数。</param>
        /// <param name="exception">导致失败的异常。</param>
        /// <param name="onDecision">决策回调，true 表示重试，false 表示不重试。</param>
        /// <param name="onRetryPolicyFailed">当重试策略本身执行失败时调用的回调。</param>
        void ShouldRetry(
            int retryCount,
            Exception exception,
            Action<bool> onDecision,
            Action<Exception> onRetryPolicyFailed);
    }
}
