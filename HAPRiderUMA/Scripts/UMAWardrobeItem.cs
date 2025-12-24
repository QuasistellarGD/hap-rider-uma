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
        if (textRecipeName != null && UMAContextBase.Instance.HasRecipe(textRecipeName))
        {
            textRecipe = UMAContextBase.Instance.GetRecipe(textRecipeName, false);
        }
        else
        {
            Debug.LogWarning("textRecipeName not found or null");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UMAWardrobeItem)), CanEditMultipleObjects]
public class UMAWardrobeItemEditor : ScriptableItemEditor
{
    UMAWardrobeItem wardrobeItem;
    public override void OnInspectorGUI()
    {
        wardrobeItem = (UMAWardrobeItem)target;

        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("textRecipeName"), new GUIContent("UMA Text Recipe Name", "The UMA Text Recipe of the Item"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textRecipe"), new GUIContent("UMA Text Recipe", "The UMA Text Recipe of the Item"));

        if(serializedObject.hasModifiedProperties)
        {
            if (UMAContextBase.Instance.HasRecipe(wardrobeItem.textRecipeName))
            {
                wardrobeItem.textRecipe = UMAContextBase.Instance.GetRecipe(wardrobeItem.textRecipeName, false);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif