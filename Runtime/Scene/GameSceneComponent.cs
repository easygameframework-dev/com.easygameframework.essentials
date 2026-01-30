using System;
using System.Linq;
using EasyGameFramework.Core.Resource;
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

        protected override void Awake()
        {
            base.Awake();
            _sceneComponent = GameEntry.GetComponent<SceneComponent>();
        }

        /// <summary>
        /// 加载游戏场景，支持可选的重试策略和进度回调。
        /// </summary>
        /// <param name="sceneAssetAddress">要加载的场景的资源地址。</param>
        /// <param name="stateChanged">加载期间状态变化的可选回调。</param>
        /// <param name="onSuccess">场景完全加载并初始化时调用的可选回调。</param>
        /// <param name="onFailure">加载或初始化失败时调用的可选回调。</param>
        /// <param name="userData">传递给场景初始化器的可选自定义数据。</param>
        /// <param name="retryPolicy">加载失败的可选重试策略。</param>
        /// <exception cref="InvalidOperationException">场景加载已在进行中时抛出。</exception>
        /// <remarks>
        /// 加载过程遵循以下顺序：
        /// 1. LoadNewScene - 以附加模式加载新场景
        /// 2. InitializeNewScene - 调用场景中所有初始化器的 ISceneInitializer.Initialize
        /// 3. UnloadPreviousScene - 卸载旧场景（如果有）
        ///
        /// 如果提供了重试策略，仅加载操作会被重试。
        /// 初始化和卸载失败不会触发重试。
        /// </remarks>
        public void LoadGameScene(
            AssetAddress sceneAssetAddress,
            Action<GameSceneLoadState> stateChanged = null,
            Action onSuccess = null,
            Action<Exception> onFailure = null,
            object userData = null,
            IRetryPolicy retryPolicy = null)
        {
            if (_isLoading)
                throw new InvalidOperationException("Scene loading already in progress.");

            _isLoading = true;

            stateChanged?.Invoke(GameSceneLoadState.LoadingNewScene);

            LoadSceneWithRetry(
                sceneAssetAddress,
                userData,
                retryPolicy,
                newScene => OnSceneLoadSuccess(sceneAssetAddress, newScene, userData, stateChanged, onSuccess, onFailure),
                exception => HandleFailure(exception, onFailure));
        }

        /// <summary>
        /// 处理场景加载成功后的流程：初始化场景，然后卸载旧场景（如有）。
        /// </summary>
        private void OnSceneLoadSuccess(
            AssetAddress sceneAssetAddress,
            Scene newScene,
            object userData,
            Action<GameSceneLoadState> stateChanged,
            Action onSuccess,
            Action<Exception> onFailure)
        {
            CurrentScene = newScene;
            stateChanged?.Invoke(GameSceneLoadState.InitializingNewScene);

            InitializeScene(
                sceneAssetAddress,
                newScene,
                userData,
                () => OnSceneInitializeSuccess(sceneAssetAddress, stateChanged, onSuccess, onFailure),
                exception => HandleFailure(exception, onFailure));
        }

        /// <summary>
        /// 处理场景初始化成功后的流程：卸载旧场景（如有）或完成加载。
        /// </summary>
        private void OnSceneInitializeSuccess(
            AssetAddress sceneAssetAddress,
            Action<GameSceneLoadState> stateChanged,
            Action onSuccess,
            Action<Exception> onFailure)
        {
            if (_previousSceneAssetAddress != null)
            {
                stateChanged?.Invoke(GameSceneLoadState.UnloadingPreviousScene);
                UnloadSceneInternal(
                    _previousSceneAssetAddress.Value,
                    () => OnSceneUnloadSuccess(sceneAssetAddress, stateChanged, onSuccess),
                    exception => HandleFailure(exception, onFailure));
            }
            else
            {
                CompleteSceneLoad(sceneAssetAddress, stateChanged, onSuccess);
            }
        }

        /// <summary>
        /// 处理旧场景卸载成功后的流程：标记加载完成。
        /// </summary>
        private void OnSceneUnloadSuccess(
            AssetAddress sceneAssetAddress,
            Action<GameSceneLoadState> stateChanged,
            Action onSuccess)
        {
            CompleteSceneLoad(sceneAssetAddress, stateChanged, onSuccess);
        }

        /// <summary>
        /// 完成场景加载流程：更新状态并触发成功回调。
        /// </summary>
        private void CompleteSceneLoad(
            AssetAddress sceneAssetAddress,
            Action<GameSceneLoadState> stateChanged,
            Action onSuccess)
        {
            _previousSceneAssetAddress = sceneAssetAddress;
            stateChanged?.Invoke(GameSceneLoadState.Completed);
            onSuccess?.Invoke();
            _isLoading = false;
        }

        /// <summary>
        /// 处理失败状态并调用失败回调。
        /// </summary>
        private void HandleFailure(Exception exception, Action<Exception> onFailure = null)
        {
            _isLoading = false;
            onFailure?.Invoke(exception);
        }

        /// <summary>
        /// 加载场景，支持可选的重试逻辑。
        /// </summary>
        private void LoadSceneWithRetry(
            AssetAddress sceneAssetAddress,
            object userData,
            IRetryPolicy retryPolicy,
            Action<Scene> onSuccess,
            Action<Exception> onFailure)
        {
            if (retryPolicy != null)
            {
                RetryUtility.Run(
                    reject =>
                    {
                        LoadScene(
                            sceneAssetAddress,
                            userData,
                            onSuccess,
                            exception =>
                            {
                                var loadException = new SceneLoadException(
                                    sceneAssetAddress,
                                    $"Failed to load scene '{sceneAssetAddress}'.",
                                    exception);
                                reject(loadException);
                            });
                    },
                    retryPolicy,
                    onFailure);
            }
            else
            {
                LoadScene(sceneAssetAddress, userData, onSuccess, exception =>
                {
                    var loadException = new SceneLoadException(
                        sceneAssetAddress,
                        $"Failed to load scene '{sceneAssetAddress}'.",
                        exception);
                    onFailure(loadException);
                });
            }
        }

        /// <summary>
        /// 加载场景并注册完成回调。
        /// </summary>
        private void LoadScene(
            AssetAddress sceneAssetAddress,
            object _,
            Action<Scene> onSuccess,
            Action<Exception> onFailure)
        {
            _sceneComponent.LoadSceneSuccess += OnLoadSceneSuccess;
            _sceneComponent.LoadSceneFailure += OnLoadSceneFailure;

            _sceneComponent.LoadScene(
                sceneAssetAddress,
                userData: new LoadSceneParameters(LoadSceneMode.Additive));

            void OnLoadSceneSuccess(object sender, LoadSceneSuccessEventArgs e)
            {
                if (e.SceneAssetAddress == sceneAssetAddress)
                {
                    _sceneComponent.LoadSceneSuccess -= OnLoadSceneSuccess;
                    _sceneComponent.LoadSceneFailure -= OnLoadSceneFailure;

                    if (e.SceneAsset is not Scene scene)
                    {
                        onFailure(new Exception($"Scene asset '{e.SceneAssetAddress}' is not a Scene."));
                        return;
                    }

                    onSuccess(scene);
                }
            }

            void OnLoadSceneFailure(object sender, LoadSceneFailureEventArgs e)
            {
                if (e.SceneAssetAddress == sceneAssetAddress)
                {
                    _sceneComponent.LoadSceneSuccess -= OnLoadSceneSuccess;
                    _sceneComponent.LoadSceneFailure -= OnLoadSceneFailure;

                    onFailure(new Exception(e.ErrorMessage));
                }
            }
        }

        /// <summary>
        /// 初始化指定场景中的所有场景初始化器。
        /// </summary>
        private void InitializeScene(
            AssetAddress sceneAssetAddress,
            Scene scene,
            object userData,
            Action onSuccess,
            Action<Exception> onFailure)
        {
            try
            {
                var initializers = scene.GetRootGameObjects()
                    .SelectMany(go => go.GetComponentsInChildren<ISceneInitializer>(true))
                    .ToArray();

                if (initializers.Length == 0)
                {
                    onSuccess();
                    return;
                }

                // 跟踪初始化进度
                int completedCount = 0;
                int totalCount = initializers.Length;
                Exception firstException = null;

                foreach (var initializer in initializers)
                {
                    initializer.Initialize(userData, OnInitializeSuccess, OnInitializeFailure);
                    continue;

                    void OnInitializeFailure(Exception exception)
                    {
                        if (firstException == null)
                            firstException = exception;
                        ++completedCount;
                        CheckCompletion();
                    }

                    void OnInitializeSuccess()
                    {
                        ++completedCount;
                        CheckCompletion();
                    }
                }

                void CheckCompletion()
                {
                    if (completedCount == totalCount)
                    {
                        if (firstException != null)
                        {
                            var initException = new SceneInitializeException(
                                sceneAssetAddress,
                                "One or more scene initializers failed.",
                                firstException);
                            onFailure(initException);
                        }
                        else
                        {
                            onSuccess();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var initException = new SceneInitializeException(
                    sceneAssetAddress,
                    "Failed to find or execute scene initializers.",
                    e);
                onFailure(initException);
            }
        }

        /// <summary>
        /// 卸载场景并注册完成回调。
        /// </summary>
        private void UnloadSceneInternal(
            AssetAddress sceneAssetAddress,
            Action onSuccess,
            Action<Exception> onFailure)
        {
            _sceneComponent.UnloadSceneSuccess += OnUnloadSceneSuccess;
            _sceneComponent.UnloadSceneFailure += OnUnloadSceneFailure;

            _sceneComponent.UnloadScene(sceneAssetAddress);

            void OnUnloadSceneSuccess(object sender, UnloadSceneSuccessEventArgs e)
            {
                if (e.SceneAssetAddress == sceneAssetAddress)
                {
                    _sceneComponent.UnloadSceneSuccess -= OnUnloadSceneSuccess;
                    _sceneComponent.UnloadSceneFailure -= OnUnloadSceneFailure;
                    onSuccess();
                }
            }

            void OnUnloadSceneFailure(object sender, UnloadSceneFailureEventArgs e)
            {
                if (e.SceneAssetAddress == sceneAssetAddress)
                {
                    _sceneComponent.UnloadSceneSuccess -= OnUnloadSceneSuccess;
                    _sceneComponent.UnloadSceneFailure -= OnUnloadSceneFailure;
                    onFailure(new Exception(e.ErrorMessage));
                }
            }
        }
    }
}
