using System;
using System.Linq;
using EasyToolkit.Core.Collections;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Essentials.Editor
{
    public class AssetReferenceDrawer : EasyValueDrawer<AssetReference>
    {
        private AssetReferenceDrawerSettingsAttribute _drawerSettings;
        private AssetMatcher _assetMatcher;

        private Func<string, bool> _packageFilter;
        private Func<string, string, bool> _groupFilter;
        private Func<string, string, string, bool> _assetFilter;
        private Func<string, string, string, string> _assetMenuPathProcessor;
        private Func<string, string, string, string> _assetDisplayProcessor;
        private string _error;

        protected override void Initialize()
        {
            try
            {
                _drawerSettings = Element.GetAttribute<AssetReferenceDrawerSettingsAttribute>();
                if (_drawerSettings != null)
                {
                    var targetType = Element.LogicalParent.CastValue().ValueEntry.ValueType;
                    var target = Element.LogicalParent.CastValue().ValueEntry.WeakSmartValue;
                    if (_drawerSettings.PackageFilter.IsNotNullOrEmpty())
                    {
                        var method = targetType.GetMethods(MemberAccessFlags.AllInstance)
                                         .FirstOrDefault(m => m.Name == _drawerSettings.PackageFilter &&
                                                              m.GetParameters().Length == 1 &&
                                                              m.GetParameters()[0].ParameterType == typeof(string))
                                     ?? throw new Exception($"Cannot find method '{_drawerSettings.PackageFilter}' in '{target}'");

                        _packageFilter = (Func<string, bool>)method.CreateDelegate(typeof(Func<string, bool>), target);
                    }

                    if (_drawerSettings.GroupFilter.IsNotNullOrEmpty())
                    {
                        var method = targetType.GetMethods(MemberAccessFlags.AllInstance)
                                         .FirstOrDefault(m => m.Name == _drawerSettings.GroupFilter &&
                                                              m.GetParameters().Length == 2 &&
                                                              m.GetParameters()[0].ParameterType == typeof(string) &&
                                                              m.GetParameters()[1].ParameterType == typeof(string))
                                     ?? throw new Exception($"Cannot find method '{_drawerSettings.GroupFilter}' in '{target}'");

                        _groupFilter = (Func<string, string, bool>)method.CreateDelegate(typeof(Func<string, string, bool>), target);
                    }

                    if (_drawerSettings.AssetFilter.IsNotNullOrEmpty())
                    {
                        var method = targetType.GetMethods(MemberAccessFlags.AllInstance)
                                         .FirstOrDefault(m => m.Name == _drawerSettings.AssetFilter &&
                                                              m.GetParameters().Length == 3 &&
                                                              m.GetParameters()[0].ParameterType == typeof(string) &&
                                                              m.GetParameters()[1].ParameterType == typeof(string) &&
                                                              m.GetParameters()[2].ParameterType == typeof(string))
                                     ?? throw new Exception($"Cannot find method '{_drawerSettings.AssetFilter}' in '{target}'");

                        _assetFilter = (Func<string, string, string, bool>)method.CreateDelegate(typeof(Func<string, string, string, bool>), target);
                    }

                    if (_drawerSettings.AssetMenuPathProcessor.IsNotNullOrEmpty())
                    {
                        var method = targetType.GetMethods(MemberAccessFlags.AllInstance)
                                         .FirstOrDefault(m => m.Name == _drawerSettings.AssetMenuPathProcessor &&
                                                              m.GetParameters().Length == 3 &&
                                                              m.GetParameters()[0].ParameterType == typeof(string) &&
                                                              m.GetParameters()[1].ParameterType == typeof(string) &&
                                                              m.GetParameters()[2].ParameterType == typeof(string))
                                     ?? throw new Exception(
                                         $"Cannot find method '{_drawerSettings.AssetMenuPathProcessor}' in '{target}'");

                        _assetMenuPathProcessor =
                            (Func<string, string, string, string>)method.CreateDelegate(typeof(Func<string, string, string, string>), target);
                    }

                    if (_drawerSettings.AssetDisplayProcessor.IsNotNullOrEmpty())
                    {
                        var method = targetType.GetMethods(MemberAccessFlags.AllInstance)
                                         .FirstOrDefault(m => m.Name == _drawerSettings.AssetDisplayProcessor &&
                                                              m.GetParameters().Length == 3 &&
                                                              m.GetParameters()[0].ParameterType == typeof(string) &&
                                                              m.GetParameters()[1].ParameterType == typeof(string) &&
                                                              m.GetParameters()[2].ParameterType == typeof(string))
                                     ?? throw new Exception(
                                         $"Cannot find method '{_drawerSettings.AssetDisplayProcessor}' in '{target}'");

                        _assetDisplayProcessor =
                            (Func<string, string, string, string>)method.CreateDelegate(typeof(Func<string, string, string, string>), target);
                    }
                }

                AssetEditorUtility.RefreshTree();
                if (_packageFilter != null || _groupFilter != null || _assetFilter != null)
                {
                    _assetMatcher = new AssetMatcher(AssetEditorUtility.Tree
                        .Where(packageNode => _packageFilter == null || _packageFilter(packageNode.Name))
                        .SelectMany(packageNode => packageNode.Groups
                            .Where(groupNode => _groupFilter == null || _groupFilter(groupNode.PackageName, groupNode.Name))
                            .SelectMany(groupNode => groupNode.Assets
                                .Where(assetNode => _assetFilter == null || _assetFilter(assetNode.PackageName, assetNode.GroupName, assetNode.Name)))
                        ).ToArray());
                }
                else
                {
                    _assetMatcher = AssetEditorUtility.DefaultMatcher;
                }
            }
            catch (Exception e)
            {
                _error = e.Message;
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(_error, MessageType.Error);
                return;
            }

            if (_assetMatcher == null)
            {
                Initialize();
            }

            var assetReference = ValueEntry.SmartValue;

            var assetPath = string.IsNullOrEmpty(assetReference.PackageName)
                ? string.Empty
                : $"{assetReference.PackageName}/{assetReference.AssetName}";

            int selectedIndex = -1;
            if (!string.IsNullOrEmpty(assetPath) &&
                !_assetMatcher.TryGetIndexByAssetPath(assetPath, out selectedIndex))
            {
                EasyEditorGUI.MessageBox($"无效资源引用：{assetPath}", MessageType.Error);
                selectedIndex = -1;
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                EasyEditorGUI.MessageBox($"资源引用不能为空", MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            var assetNodes = _assetMatcher.Nodes;
            var selectedAssetNode = selectedIndex != -1 ? assetNodes[selectedIndex] : null;

            var display = new GUIContent();
            if (selectedAssetNode != null)
            {
                if (_assetDisplayProcessor != null)
                {
                    display.text = _assetDisplayProcessor(selectedAssetNode.PackageName, selectedAssetNode.GroupName, selectedAssetNode.Name);
                }
                else
                {
                    display.text = selectedAssetNode.AssetPath;
                }
            }

            selectedIndex = EasyEditorGUI.ValueDropdown(label, display, selectedIndex, assetNodes,
                (index, assetNode) => _assetMenuPathProcessor != null
                    ? new GUIContent(_assetMenuPathProcessor(assetNode.PackageName, assetNode.GroupName, assetNode.Name))
                    : new GUIContent(assetNode.MenuPath));

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

                ValueEntry.SmartValue = assetReference;
                ValueEntry.MarkDirty();
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh").SetTooltip("刷新"), GUILayout.Width(30)))
            {
                _drawerSettings = null;
                _assetMatcher = null;
                _packageFilter = null;
                _groupFilter = null;
                _assetFilter = null;
                _assetMenuPathProcessor = null;
                _assetDisplayProcessor = null;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
