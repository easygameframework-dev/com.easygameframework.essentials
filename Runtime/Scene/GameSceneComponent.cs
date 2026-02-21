using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using EasyGameFramework.Core.Resource;
using EasyGameFramework.Tasks;
using UnityEngine.SceneManagement;

namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 提供带有自动场景初始化的高级场景加载功能。
    /// 处理完整的场景生命周期：加载 → 初始化 → 卸载旧场景。
    /// </summary>
    public class GameSceneComponent : GameFrameworkComponent
    {
        private SceneComponent _sceneComponent;
        private AssetAddress? _previousSceneAssetAddress;
        private bool _isLoading;

        /// <summary>
        /// 获取当前活动的游戏场景。
        /// </summary>
        public Scene CurrentScene { get; private set; }

        private void Start()
        {
            _sceneComponent = GameEntry.GetComponent<SceneComponent>();
        }

        public async UniTask LoadGameSceneAsync(
            AssetAddress sceneAssetAddress,
            Action<GameSceneLoadState> stateChanged = null,
            IRetryPolicy retryPolicy = null)
        {
            if (_isLoading)
                throw new InvalidOperationException("Scene loading already in progress.");

            _isLoading = true;

            stateChanged?.Invoke(GameSceneLoadState.LoadingNewScene);

            var scene = await LoadSceneWithRetryAsync(
                sceneAssetAddress,
                retryPolicy);

            await InitializeSceneAsync(sceneAssetAddress, scene);

            if (_previousSceneAssetAddress != null)
            {
                await _sceneComponent.UnloadSceneAsync(_previousSceneAssetAddress.Value);
            }

            _previousSceneAssetAddress = sceneAssetAddress;
            CurrentScene = scene;

            _isLoading = false;
        }

        /// <summary>
        /// 加载场景，支持可选的重试逻辑。
        /// </summary>
        private UniTask<Scene> LoadSceneWithRetryAsync(
            AssetAddress sceneAssetAddress,
            IRetryPolicy retryPolicy)
        {
            if (retryPolicy != null)
            {
                return RetryUtility.RunAsync(
                    () => _sceneComponent.LoadSceneAsync(
                        sceneAssetAddress, LoadSceneMode.Additive),
                    retryPolicy);
            }
            else
            {
                return _sceneComponent.LoadSceneAsync(
                    sceneAssetAddress, LoadSceneMode.Additive);
            }
        }

        /// <summary>
        /// 初始化指定场景中的所有场景初始化器。
        /// </summary>
        private async UniTask InitializeSceneAsync(
            AssetAddress sceneAssetAddress,
            Scene scene)
        {
            var initializers = scene.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<ISceneInitializer>(true))
                .ToArray();

            try
            {
                await UniTask.WhenAll(initializers.Select(initializer => initializer.InitializeAsync(sceneAssetAddress)));
            }
            catch (Exception e)
            {
                throw new SceneInitializeException(
                    sceneAssetAddress,
                    "Failed to execute scene initializers.",
                    e);
            }
        }
    }
}
