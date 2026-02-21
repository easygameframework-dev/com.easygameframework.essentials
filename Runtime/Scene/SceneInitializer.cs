using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 定义可以在场景加载时初始化场景的组件。
    /// </summary>
    public interface ISceneInitializer
    {
        UniTask InitializeAsync(object userData);
    }
}
