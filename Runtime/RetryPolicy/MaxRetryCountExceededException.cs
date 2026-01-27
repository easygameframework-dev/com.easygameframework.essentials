using System;
using System.Collections.Generic;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 当操作重试次数超过最大允许值时抛出的异常。
    /// </summary>
    public sealed class MaxRetryCountExceededException : Exception
    {
        /// <summary>
        /// 获取最大重试次数。
        /// </summary>
        public int MaxRetryCount { get; }

        /// <summary>
        /// 获取重试过程中发生的所有异常。
        /// </summary>
        public IReadOnlyList<Exception> Exceptions { get; }

        /// <summary>
        /// 初始化 <see cref="MaxRetryCountExceededException"/> 类的新实例。
        /// </summary>
        /// <param name="maxRetryCount">最大重试次数。</param>
        /// <param name="exceptions">重试过程中发生的所有异常。</param>
        public MaxRetryCountExceededException(int maxRetryCount, IReadOnlyList<Exception> exceptions)
            : base($"Operation failed after exceeding the maximum retry count of {maxRetryCount}.")
        {
            MaxRetryCount = maxRetryCount;
            Exceptions = exceptions ?? Array.Empty<Exception>();
        }

        /// <summary>
        /// 初始化 <see cref="MaxRetryCountExceededException"/> 类的新实例。
        /// </summary>
        /// <param name="maxRetryCount">最大重试次数。</param>
        /// <param name="exceptions">重试过程中发生的所有异常。</param>
        /// <param name="innerException">导致当前异常的内部异常。</param>
        public MaxRetryCountExceededException(int maxRetryCount, IReadOnlyList<Exception> exceptions,
            Exception innerException)
            : base($"Operation failed after exceeding the maximum retry count of {maxRetryCount}.", innerException)
        {
            MaxRetryCount = maxRetryCount;
            Exceptions = exceptions ?? Array.Empty<Exception>();
        }
    }
}
