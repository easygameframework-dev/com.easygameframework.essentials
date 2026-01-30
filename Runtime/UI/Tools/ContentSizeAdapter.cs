using System;
using System.Collections.Generic;
using EasyToolkit.Core.Mathematics;
using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    [ExecuteAlways]
    [EasyInspector]
    [AddComponentMenu("Layout/Content Size Adapter")]
    public class ContentSizeAdapter : MonoBehaviour
    {
        enum MatchModes
        {
            [LabelText("宽度")]
            Width,
            [LabelText("高度")]
            Height
        }

        [Serializable]
        [HideLabel]
        class BindingItem
        {
            [Required]
            [LabelText("绑定对象")]
            public RectTransform Target;
            [Range(0f, 2f)]
            [LabelText("系数")]
            public float Coefficient = 1f;
            [LabelText("匹配模式")]
            public MatchModes MatchMode = MatchModes.Width;

            [LabelText("原大小")]
            [ReadOnly]
            public Vector2 OriginSize;
        }

        [Required]
        [LabelText("目标对象")]
        [SerializeField] private RectTransform _target;

        [LabelText("绑定列表")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<BindingItem> _bindingItems = new List<BindingItem>();

        private Vector2 _originSize;


        private void Start()
        {
            _originSize = _target.sizeDelta;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                foreach (var item in _bindingItems)
                {
                    if (item.Target == null)
                        continue;

                    var size = new Vector2(
                        item.Target.sizeDelta.x.IsApproximatelyOf(0f) ? item.OriginSize.x : item.Target.sizeDelta.x,
                        item.Target.sizeDelta.y.IsApproximatelyOf(0f) ? item.OriginSize.y : item.Target.sizeDelta.y);

                    if (size != item.OriginSize)
                    {
                        item.OriginSize = size;
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.SetDirty(this);
#endif
                    }
                }
            }
            else
            {
                Vector2 totalDifferentSize = Vector2.zero;
                foreach (var item in _bindingItems)
                {
                    var differentSize = item.Target.sizeDelta - item.OriginSize;
                    differentSize *= item.Coefficient;
                    totalDifferentSize += item.MatchMode == MatchModes.Width
                        ? new Vector2(differentSize.x, 0)
                        : new Vector2(0, differentSize.y);
                }

                _target.sizeDelta = _originSize + totalDifferentSize;
            }
        }
    }
}
