using System.Collections.Generic;

namespace EasyGameFramework.Essentials.Editor
{
    public class PackageNode
    {
        public string Name { get; }
        public List<GroupNode> Groups { get; } = new List<GroupNode>();

        public PackageNode(string name)
        {
            Name = name;
        }
    }
}
