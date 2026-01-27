using System.Collections.Generic;

namespace EasyGameFramework.Essentials.Editor
{
    public class GroupNode
    {
        public PackageNode Parent { get; }

        public string Name { get; }
        public List<AssetNode> Assets { get; } = new List<AssetNode>();

        public string PackageName => Parent.Name;
        public string Path => $"{PackageName}/{Name}";

        public GroupNode(PackageNode parent, string name)
        {
            Parent = parent;
            Name = name;
        }
    }
}
