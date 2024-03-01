using MalbersAnimations;
using MalbersAnimations.InventorySystem;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Malbers Inventory/New UMA Clothing Item")]
[JsonObject(MemberSerialization.OptIn)]
public class UMAWardrobeItem : Item
{
    [Tooltip("UMA Clothing recipe")]
    [JsonProperty]
    public string textRecipeName;
    [HideInInspector]
    public UMATextRecipe textRecipe;
    [HideInInspector]
    public InventorySlot inventorySlot; // set when item is equipped

    void Start()
    {
        if (UMAContextBase.Instance.HasRecipe(textRecipeName))
        {
            textRecipe = UMAContextBase.Instance.GetRecipe(textRecipeName, false);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UMAWardrobeItem)), CanEditMultipleObjects]
public class UMAWardrobeItemEditor : ScriptableItemEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("textRecipeName"), new GUIContent("UMA Text Recipe Name", "The UMA Text Recipe of the Item"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif