using System;

namespace EasyGameFramework.Essentials
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AssetReferenceDrawerSettingsAttribute : Attribute
    {
        public string PackageFilter { get; set; }
        public string GroupFilter { get; set; }
        public string AssetFilter { get; set; }
        public string AssetMenuPathProcessor { get; set; }
        public string AssetDisplayProcessor { get; set; }
    }
}
