using System;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 定义可以在场景加载时初始化场景的组件。
    /// </summary>
    public interface ISceneInitializer
    {
        /// <summary>
        /// 使用指定的用户数据初始化场景。
        /// </summary>
        /// <param name="userData">用于初始化的自定义用户数据。</param>
        /// <param name="onSuccess">初始化成功完成时调用。</param>
        /// <param name="onFailure">初始化失败时调用，包含异常信息。</param>
        void Initialize(object userData, Action onSuccess, Action<Exception> onFailure);
    }

    /// <summary>
    /// 场景初始化组件的基类。
    /// 继承此类以实现自定义场景初始化逻辑。
    /// </summary>
    /// <remarks>
    /// 重写 OnInitialize 方法以提供自定义初始化逻辑。
    /// 默认实现立即完成。
    /// </remarks>
    public class SceneInitializer : MonoBehaviour, ISceneInitializer
    {
        /// <summary>
        /// 使用指定的用户数据初始化场景。
        /// </summary>
        /// <param name="userData">用于初始化的自定义用户数据。</param>
        /// <param name="onSuccess">初始化成功完成时调用。</param>
        /// <param name="onFailure">初始化失败时调用，包含异常信息。</param>
        void ISceneInitializer.Initialize(object userData, Action onSuccess, Action<Exception> onFailure)
        {
            try
            {
                OnInitialize(userData);
                onSuccess?.Invoke();
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e);
            }
        }

        /// <summary>
        /// 重写此方法以实现自定义场景初始化逻辑。
        /// </summary>
        /// <param name="userData">用于初始化的自定义用户数据。</param>
        /// <remarks>
        /// 此方法在场景加载时调用。
        /// 抛出异常以表示初始化失败。
        /// 对于异步操作，显式实现 ISceneInitializer 并使用协程。
        /// </remarks>
        protected virtual void OnInitialize(object userData)
        {
            // 默认实现：无需初始化
        }
    }
}
