using System.Collections.Generic;
using EasyToolkit.Core.Patterns;
using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    [EasyInspector]
    [ScriptableObjectSingletonConfiguration("Assets/Resources/Configs/UI")]
    public class UIMessageBoxConfigAsset : ScriptableObjectSingleton<UIMessageBoxConfigAsset>
    {
        [Title("资源")]
        [LabelText("资源引用")]
        [SerializeField] private AssetReference _assetReference = new("EasyGameFramework", "UI_MessageBox");

        [Title("UIMessageType文本")]
        [SerializeField] private string _tipTitle = "提示";
        [SerializeField] private string _warnTitle = "警告";
        [SerializeField] private string _errorTitle = "错误";
        [SerializeField] private string _fatalTitle = "严重错误";

        [Title("UIMessageBoxButtons文本")]
        [LabelText("OK")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _okTexts = new List<string>(){ "确认" };

        [LabelText("OKCancel")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _okCancelTexts = new List<string>(){ "确认", "取消" };

        [LabelText("Yes")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _yesTexts = new List<string>(){ "是" };

        [LabelText("YesNo")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _yesNoTexts = new List<string>(){ "是", "否" };

        [LabelText("YesNoCancel")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _yesNoCancelTexts = new List<string>(){ "是", "否", "取消" };


        public AssetReference AssetReference => _assetReference;

        public string TipTitle => _tipTitle;
        public string WarnTitle => _warnTitle;
        public string ErrorTitle => _errorTitle;
        public string FatalTitle => _fatalTitle;

        public IReadOnlyList<string> OkTexts => _okTexts;
        public IReadOnlyList<string> OkCancelTexts => _okCancelTexts;
        public IReadOnlyList<string> YesTexts => _yesTexts;
        public IReadOnlyList<string> YesNoTexts => _yesNoTexts;
        public IReadOnlyList<string> YesNoCancelTexts => _yesNoCancelTexts;
    }
}
