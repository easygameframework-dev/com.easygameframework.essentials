using System;
using EasyToolKit.Core.Pooling;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    public class GameObjectPoolComponent : GameFrameworkComponent
    {
        private IGameObjectPoolManager _manager;

        protected override void Awake()
        {
            base.Awake();
            _manager = PoolManagerFactory.CreateGameObjectPoolManager(transform);
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="original">用于实例化的原始游戏对象</param>
        /// <exception cref="InvalidOperationException">当已存在同名同类型的池时抛出</exception>
        public IGameObjectPool CreatePool(string poolName, GameObject original)
        {
            return _manager.CreatePool(poolName, original);
        }

        public bool HasPool(string poolName)
        {
            return _manager.HasPool(poolName);
        }

        /// <summary>
        /// 获取指定名称和类型的对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <returns>找到的对象池</returns>
        /// <exception cref="InvalidOperationException">当找不到指定的对象池时抛出</exception>
        public IGameObjectPool GetPool(string poolName)
        {
            return _manager.GetPool(poolName);
        }

        public IGameObjectPool GetOrCreatePool(string poolName, GameObject original)
        {
            if (_manager.HasPool(poolName))
            {
                return GetPool(poolName);
            }
            else
            {
                return CreatePool(poolName, original);
            }
        }
    }
}
