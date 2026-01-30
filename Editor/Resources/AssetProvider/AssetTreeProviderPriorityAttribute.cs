using System;
using EasyToolkit.Core.Mathematics;

namespace EasyGameFramework.Essentials.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AssetTreeProviderPriorityAttribute : Attribute
    {
        public OrderPriority Priority { get; }

        public AssetTreeProviderPriorityAttribute(double priority)
        {
            Priority = priority;
        }
    }
}
