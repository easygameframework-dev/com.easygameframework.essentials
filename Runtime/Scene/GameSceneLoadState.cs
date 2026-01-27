namespace EasyGameFramework.Essentials
{
    /// <summary>
    /// 定义游戏场景加载过程的状态。
    /// </summary>
    public enum GameSceneLoadState
    {
        /// <summary>
        /// 正在加载新场景。
        /// </summary>
        LoadingNewScene,

        /// <summary>
        /// 正在初始化新场景。
        /// </summary>
        InitializingNewScene,

        /// <summary>
        /// 正在卸载旧场景。
        /// </summary>
        UnloadingPreviousScene,

        /// <summary>
        /// 场景加载和初始化完成。
        /// </summary>
        Completed,
    }
}
