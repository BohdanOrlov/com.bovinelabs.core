﻿// <copyright file="UITemplate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Editor.UI
{
    using Unity.Entities.Editor;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// Manages loading a pair of uxml/uss files in the same folder.
    /// Roughly based off Unity.Entities.Editor.UITemplate but quite stripped down for own uses.
    /// </summary>
    public readonly struct UITemplate
    {
        private readonly string uxmlPath;
        private readonly string ussPath;

        public UITemplate(string path)
        {
            this.uxmlPath = $"{path}.uxml";
            this.ussPath = $"{path}.uss";
        }

        private VisualTreeAsset Template => EditorGUIUtility.Load(this.uxmlPath) as VisualTreeAsset;

        private StyleSheet StyleSheet => AssetDatabase.LoadAssetAtPath<StyleSheet>(this.ussPath);

        /// <summary> Clones the template into the given root element and applies the style sheets from the template. </summary>
        /// <param name="root">The element that will serve as the root for cloning the template.</param>
        /// <returns> Returns the updated root for convenience. </returns>
        public VisualElement Clone(VisualElement root = null)
        {
            root = this.CloneTemplate(root);
            this.AddStyleSheetSkinVariant(root);
            return root;
        }

        private VisualElement CloneTemplate(VisualElement element = null)
        {
            if (this.Template == null)
            {
                return element;
            }

            if (element == null)
            {
                return this.Template.CloneTree();
            }

            this.Template.CloneTree(element);
            return element;
        }

        private void AddStyleSheetSkinVariant(VisualElement element)
        {
            if (this.StyleSheet == null)
            {
                return;
            }

            if (element == null)
            {
                return;
            }

            element.styleSheets.Add(this.StyleSheet);
            var assetPath = AssetDatabase.GetAssetPath(this.StyleSheet);
            assetPath = assetPath.Insert(assetPath.LastIndexOf('.'), Resources.SkinSuffix);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath) is var skin && skin != null)
            {
                element.styleSheets.Add(skin);
            }
        }
    }
}