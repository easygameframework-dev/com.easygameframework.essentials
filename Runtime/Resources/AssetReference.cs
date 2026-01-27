using System;
using EasyGameFramework.Core.Resource;
using EasyToolKit.Inspector.Attributes;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    [Serializable, ReferenceObjectDrawerSettings(HideFoldout = true)]
    public class AssetReference : IEquatable<AssetReference>
    {
        [SerializeField] private string _packageName;
        [SerializeField] private string _assetName;

        public string PackageName
        {
            get => _packageName;
            set => _packageName = value;
        }

        public string AssetName
        {
            get => _assetName;
            set => _assetName = value;
        }

        public AssetReference(AssetAddress assetAddress)
        {
            _packageName = assetAddress.PackageName;
            _assetName = assetAddress.Location;
        }

        public AssetReference(string packageName, string assetName)
        {
            _packageName = packageName;
            _assetName = assetName;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(_packageName) && !string.IsNullOrEmpty(_assetName);
        }

        public override string ToString()
        {
            return $"{_packageName}/{_assetName}";
        }

        public bool Equals(AssetReference other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _packageName == other._packageName && _assetName == other._assetName;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AssetReference)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_packageName, _assetName);
        }
    }
}
