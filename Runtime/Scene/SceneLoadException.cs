using System;
using EasyGameFramework.Core.Resource;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 场景加载失败时抛出的异常。
    /// </summary>
    public class SceneLoadException : Exception
    {
        /// <summary>
        /// 获取加载失败的场景的资源地址。
        /// </summary>
        public AssetAddress SceneAssetAddress { get; }

        /// <summary>
        /// 初始化 SceneLoadException 类的新实例。
        /// </summary>
        /// <param name="sceneAssetAddress">加载失败的场景的资源地址。</param>
        /// <param name="message">错误消息。</param>
        /// <param name="inner">内部异常。</param>
        public SceneLoadException(AssetAddress sceneAssetAddress, string message, Exception inner = null)
            : base(message, inner)
        {
            SceneAssetAddress = sceneAssetAddress;
        }
    }
}
