﻿// <copyright file="ObjectDefinition.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_OBJECT_DEFINITION
namespace BovineLabs.Core.ObjectManagement
{
    using BovineLabs.Core.PropertyDrawers;
    using JetBrains.Annotations;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    /// <summary>
    /// ObjectDefinition provides an auto generated map of an automatically managed, auto incremented and branch safe key to an entity.
    /// This mapping is stored in <see cref="ObjectDefinitionRegistry"/> where the key is the index in the dynamic buffer.
    /// It provides a way to give high definition <see cref="ObjectCategories"/> which can auto place into <see cref="ObjectGroup"/> for you.
    /// </summary>
    [UIDManager("ObjectManagementSettings", "objectDefinitions")]
    [AssetCreator("object.definitions", "Assets/Configs/Definitions", "Definition.asset")]
    public sealed class ObjectDefinition : ScriptableObject, IUID
    {
        [InspectorReadOnly]
        [SerializeField]
        private ObjectId id;

        [SerializeField]
        private string friendlyName = string.Empty;

        [SerializeField]
        [UsedImplicitly]
        private string description = string.Empty;

        [ObjectCategories]
        [SerializeField]
        private int categories;

        [SerializeField]
        private GameObject? prefab;

        public GameObject? Prefab
        {
            get => this.prefab;
            internal set => this.prefab = value;
        }

        public ObjectCategory Categories => new ObjectCategory { Value = (uint)this.categories };

        public string FriendlyName => this.friendlyName;

        public ObjectId ID => this.id;

        int IUID.ID
        {
            get => this.id;
            set => this.id = new ObjectId { ID = value };
        }

        public static implicit operator ObjectId(ObjectDefinition definition)
        {
            return definition != null ? definition.id : default;
        }

#if UNITY_EDITOR
        private ObjectCategories? objectCategories;

        private void OnValidate()
        {
            if (this.objectCategories == null)
            {
                this.objectCategories = Resources.Load<ObjectCategories>(nameof(ObjectCategories));
            }

            if (this.objectCategories == null)
            {
                return;
            }

            foreach (var c in this.objectCategories.Components)
            {
                if (!c.ObjectGroup.Id.IsValid)
                {
                    continue;
                }

                var objectGroup = AssetDatabase.LoadAssetAtPath<ObjectGroup>(AssetDatabase.GUIDToAssetPath(c.ObjectGroup.Id.GlobalId.AssetGUID));
                var serializedObject = new SerializedObject(objectGroup);
                var serializedProperty = serializedObject.FindProperty("definitions");

                var index = IndexOf(serializedProperty, this);
                bool changed = false;

                if ((this.categories & (1 << c.Value)) != 0)
                {
                    if (index == -1)
                    {
                        index = serializedProperty.arraySize;
                        serializedProperty.InsertArrayElementAtIndex(index);
                        serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = this;
                        changed = true;
                    }
                }
                else
                {
                    if (index != -1)
                    {
                        serializedProperty.DeleteArrayElementAtIndex(index);
                        changed = true;
                    }
                }

                if (changed)
                {
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    AssetDatabase.SaveAssetIfDirty(c.ObjectGroup.Id.GlobalId.AssetGUID);
                }
            }
        }

        private static int IndexOf(SerializedProperty property, ObjectDefinition objectDefinition)
        {
            for (var i = 0; i < property.arraySize; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == objectDefinition)
                {
                    return i;
                }
            }

            return -1;
        }
#endif
    }
}
#endif