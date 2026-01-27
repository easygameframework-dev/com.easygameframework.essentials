using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Mathematics;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(AssetReference), true)]
    public class AssetReferenceUnityDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!AssetEditorUtility.IsCacheValid)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var assetReference = (AssetReference)property.boxedValue;

            var assetPath = string.IsNullOrEmpty(assetReference.PackageName)
                ? string.Empty
                : $"{assetReference.PackageName}/{assetReference.AssetName}";
            if ((!string.IsNullOrEmpty(assetPath) &&
                 !AssetEditorUtility.DefaultMatcher.TryGetIndexByAssetPath(assetPath, out _)) ||
                string.IsNullOrEmpty(assetPath))
            {
                var size = EasyEditorGUI.CalculateMessageBoxSize($"无效资源引用：{assetPath}", MessageType.Error);
                return EditorGUIUtility.singleLineHeight + size.y;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assetReference = (AssetReference)property.boxedValue;

            var assetPath = string.IsNullOrEmpty(assetReference.PackageName)
                ? string.Empty
                : $"{assetReference.PackageName}/{assetReference.AssetName}";

            int selectedIndex = -1;
            if (!string.IsNullOrEmpty(assetPath) &&
                !AssetEditorUtility.DefaultMatcher.TryGetIndexByAssetPath(assetPath, out selectedIndex))
            {
                MessageBox($"无效资源引用：{assetPath}", MessageType.Error);
                selectedIndex = -1;
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                MessageBox($"资源引用不能为空", MessageType.Error);
            }

            EditorGUI.BeginChangeCheck();

            var assetNodes = AssetEditorUtility.DefaultMatcher.Nodes;
            position.height = EditorGUIUtility.singleLineHeight;
            var selectedAssetNode = selectedIndex != -1 ? assetNodes[selectedIndex] : null;

            var display = new GUIContent();
            if (selectedAssetNode != null)
            {
                display.text = selectedAssetNode.AssetPath;
            }

            selectedIndex = EasyEditorGUI.ValueDropdown(
                position.WithXMaxOffsetBy(-30), label, display,
                selectedIndex, assetNodes,
                (index, assetInfo) => new GUIContent(assetInfo.MenuPath));

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedIndex != -1)
                {
                    selectedAssetNode = assetNodes[selectedIndex];
                    assetReference.PackageName = selectedAssetNode.PackageName;
                    assetReference.AssetName = selectedAssetNode.Name;
                }
                else
                {
                    assetReference.PackageName = string.Empty;
                    assetReference.AssetName = string.Empty;
                }

                property.boxedValue = assetReference;
            }

            if (GUI.Button(position.WithXMaxOffsetBy(-30),
                    EditorGUIUtility.IconContent("d_Refresh").SetTooltip("刷新")))
            {
                AssetEditorUtility.RefreshTree();
            }

            void MessageBox(string message, MessageType messageType)
            {
                var size = EasyEditorGUI.CalculateMessageBoxSize(message, messageType);
                var rect = position.WithHeight(size.y);
                EasyEditorGUI.MessageBox(rect, message, messageType);
                position.y += size.y;
            }
        }
    }
}
