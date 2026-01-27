using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyGameFramework.Essentials.Editor
{
    public static class AssetEditorUtility
    {
        private static PackageNode[] s_treeCache;
        private static AssetMatcher s_defaultMatcherCache;

        public static bool IsCacheValid => s_treeCache != null;

        public static PackageNode[] Tree
        {
            get
            {
                EnsureInitialize();
                return s_treeCache;
            }
        }

        public static AssetMatcher DefaultMatcher
        {
            get
            {
                if (s_defaultMatcherCache == null)
                {
                    s_defaultMatcherCache = new AssetMatcher(Tree
                        .SelectMany(p => p.Groups.SelectMany(g => g.Assets))
                        .ToArray());
                }

                return s_defaultMatcherCache;
            }
        }

        public static void EnsureInitialize()
        {
            if (s_treeCache == null)
            {
                RefreshTree();
            }
        }

        public static void ClearCache()
        {
            s_treeCache = null;
            s_defaultMatcherCache = null;
        }

        public static void RefreshTree()
        {
            ClearCache();
            s_treeCache = AssetTreeProviderFactory.GetTreeProvider().GetTree();
        }
    }
}
