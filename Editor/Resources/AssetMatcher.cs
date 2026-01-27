using System.Collections.Generic;

namespace EasyGameFramework.Essentials.Editor
{
    public class AssetMatcher
    {
        private readonly Dictionary<string, int> _indexByAssetPath = new Dictionary<string, int>();

        public AssetNode[] Nodes { get; }

        public AssetMatcher(AssetNode[] nodes)
        {
            Nodes = nodes;

            for (int i = 0; i < Nodes.Length; i++)
            {
                _indexByAssetPath.Add(Nodes[i].AssetPath, i);
            }
        }

        public bool TryGetIndexByAssetPath(string assetPath, out int index)
        {
            return _indexByAssetPath.TryGetValue(assetPath, out index);
        }
    }
}
