using System;
using System.Linq;
using System.Reflection;
using EasyToolkit.Core.Mathematics;
using EasyToolkit.Core.Reflection;

namespace EasyGameFramework.Essentials.Editor
{
    public static class AssetTreeProviderFactory
    {
        private static readonly Type[] ProviderTypes;

        static AssetTreeProviderFactory()
        {
            ProviderTypes = AssemblyUtility.GetTypes(AssemblyCategory.Custom)
                .Where(t => typeof(IAssetTreeProvider).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .OrderByDescending(GetProviderPriority)
                .ToArray();
        }

        public static IAssetTreeProvider GetTreeProvider()
        {
            if (ProviderTypes.Length == 0)
            {
                return null;
            }
            return ProviderTypes[0].CreateInstance<IAssetTreeProvider>();
        }

        private static OrderPriority GetProviderPriority(Type providerType)
        {
            var priorityAttribute = providerType.GetCustomAttribute<AssetTreeProviderPriorityAttribute>();
            return priorityAttribute?.Priority ?? OrderPriority.Default;
        }
    }
}
