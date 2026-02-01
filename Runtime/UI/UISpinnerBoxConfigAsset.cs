using EasyToolkit.Core.Patterns;
using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    [EasyInspector]
    [ScriptableObjectSingletonConfiguration("Assets/Resources/Configs/UI")]
    public class UISpinnerBoxConfigAsset : ScriptableObjectSingleton<UISpinnerBoxConfigAsset>
    {
        [Title("资源")]
        [LabelText("资源引用")]
        [SerializeField] private AssetReference _assetReference = new("EasyGameFramework", "UI_SpinnerBox");

        public AssetReference AssetReference => _assetReference;
    }
}
