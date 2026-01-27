namespace EasyGameFramework.Essentials.Editor
{
    public class AssetNode
    {
        public GroupNode Parent { get; }

        public string Name { get; }

        public string PackageName => Parent.PackageName;

        public string GroupName => Parent.Name;

        public string AssetPath => $"{PackageName}/{Name}";

        public string MenuPath => $"{Parent.Path}/{Name}";

        public AssetNode(GroupNode parent, string name)
        {
            Parent = parent;
            Name = name;
        }
    }
}
