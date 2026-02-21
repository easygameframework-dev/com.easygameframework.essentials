using System;
using Cysharp.Threading.Tasks;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 定义失败操作的重试策略。
    /// </summary>
    public interface IRetryPolicy
    {
        UniTask<bool> ShouldRetryAsync(Exception exception);
    }
}
