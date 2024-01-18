using MalbersAnimations;
using MalbersAnimations.InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Malbers Inventory/New UMA Clothing Item")]
public class UMAWardrobeItem : Item
{
    [Tooltip("UMA Clothing recipe")]
    public UMATextRecipe textRecipe;
    [HideInInspector]
    public string wardrobeSlot;
    [HideInInspector]
    public InventorySlot inventorySlot; // set when item is equipped

    void Start()
    {
    }

    private void OnEnable()
    {
        wardrobeSlot = textRecipe.wardrobeSlot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UMAWardrobeItem)), CanEditMultipleObjects]
public class ScriptableItemEditor : Editor
{
    private SerializedProperty ID, itemIcon, itemName, itemDescription, subtitleText, Stackable, inWorldPrefab, textRecipe,
        Equippable, Usable, Droppable, Discardable, UseReaction, EquipReaction, UnequipReaction, DropReaction, RemoveReaction,
        itemType, maxStacks, OnItemUsed, OnItemEquipped, OnItemUnEquipped, OnItemDropped, OnItemRemoved
        , Editor_Tabs1, rarityOfItem, SellValueOfItem, NotSellable;


    private static readonly string[] tab1 = new string[] { "General", "Reactions", "Events" };
    private void OnEnable()
    {
        ID = serializedObject.FindProperty("ID");

        Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");

        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("icon");
        itemType = serializedObject.FindProperty("type");
        itemDescription = serializedObject.FindProperty("description");
        subtitleText = serializedObject.FindProperty("subtitleText");
        maxStacks = serializedObject.FindProperty("maxStacks");
        Stackable = serializedObject.FindProperty("Stackable");
        inWorldPrefab = serializedObject.FindProperty("inWorldPrefab");
        textRecipe = serializedObject.FindProperty("textRecipe");
        Equippable = serializedObject.FindProperty("Equippable");
        Usable = serializedObject.FindProperty("Usable");
        Droppable = serializedObject.FindProperty("Droppable");
        Discardable = serializedObject.FindProperty("Discardable");
        UseReaction = serializedObject.FindProperty("UseReaction");
        EquipReaction = serializedObject.FindProperty("EquipReaction");
        UnequipReaction = serializedObject.FindProperty("UnequipReaction");
        DropReaction = serializedObject.FindProperty("DropReaction");
        RemoveReaction = serializedObject.FindProperty("RemoveReaction");
        OnItemUsed = serializedObject.FindProperty("OnItemUsed");
        OnItemEquipped = serializedObject.FindProperty("OnItemEquipped");
        OnItemUnEquipped = serializedObject.FindProperty("OnItemUnEquipped");
        OnItemDropped = serializedObject.FindProperty("OnItemDropped");
        OnItemRemoved = serializedObject.FindProperty("OnItemRemoved");
        rarityOfItem = serializedObject.FindProperty("rarityOfItem");
        SellValueOfItem = serializedObject.FindProperty("SellValueOfItem");
        NotSellable = serializedObject.FindProperty("NotSellable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, tab1);
        int Selection = Editor_Tabs1.intValue;

        switch (Selection)
        {
            case 0: ShowGeneral(); break;
            case 1: ShowReactions(); break;
            case 2: ShowEvents(); break;

            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
    private void ShowGeneral()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            MalbersEditor.DrawDescription("Inventory Item Data");
            EditorGUILayout.PropertyField(ID);
            EditorGUILayout.PropertyField(itemName, new GUIContent("Name", "The name of the item"));
            EditorGUILayout.PropertyField(rarityOfItem, new GUIContent("Rarity", "The rarity of the item"));
            EditorGUILayout.PropertyField(NotSellable, new GUIContent("Not Sellable", "Mainly used for Quest Items"));
            if (NotSellable.boolValue == false)
            {
                EditorGUILayout.PropertyField(SellValueOfItem, new GUIContent("Sell Value", "The Sell Value of the Item"));
            }
            EditorGUILayout.PropertyField(itemType, new GUIContent("Type", "The Inventory Type of the item"));
            EditorGUILayout.PropertyField(itemIcon, new GUIContent("Icon", "The Inventory Icon of the item"));
            EditorGUILayout.PropertyField(inWorldPrefab, new GUIContent("In World Prefab", "The Prefab of the Item"));
            EditorGUILayout.PropertyField(textRecipe, new GUIContent("UMA Text Recipe", "The UMA Text Recipe of the Item"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(subtitleText);
            EditorGUILayout.PropertyField(itemDescription);
        }
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {

            using (new EditorGUI.DisabledGroupScope(true))
            {
                ItemType itemTypeTemp = (ItemType)itemType.objectReferenceValue;
                if (itemTypeTemp != null)
                {
                    EditorGUILayout.PropertyField(Stackable);
                    Stackable.boolValue = itemTypeTemp.Stackable;
                }

            }

            if (Stackable.boolValue == true)
            {
                EditorGUILayout.PropertyField(maxStacks);
            }

            EditorGUILayout.PropertyField(Equippable);
            EditorGUILayout.PropertyField(Usable);
            EditorGUILayout.PropertyField(Droppable);
            EditorGUILayout.PropertyField(Discardable);

            #region oldCode
            //There's no Need to  Disable Stackable


            //ItemType itemTypeTemp = (ItemType)itemType.objectReferenceValue;
            //if (itemTypeTemp != null)
            //{
            //    switch (itemTypeTemp.ID)
            //    {
            //        case 2: //Weapon
            //            using (new EditorGUI.DisabledGroupScope(true))
            //            {
            //                EditorGUILayout.PropertyField(Stackable, new GUIContent("Stackable", "Is the item stackable? NO for Weapons or Armour"));
            //            }
            //            EditorGUILayout.PropertyField(Equippable, new GUIContent("Equippable", "Is the item equippable?"));
            //            EditorGUILayout.PropertyField(Usable, new GUIContent("Usable", "Is the item usable?"));
            //            EditorGUILayout.PropertyField(Droppable, new GUIContent("Droppable", "Is the item droppable?"));
            //            EditorGUILayout.PropertyField(Discardable, new GUIContent("Discardable", "Is the item discardable/removable?"));
            //            break;
            //        case 3: //Armour
            //            using (new EditorGUI.DisabledGroupScope(true))
            //            {
            //                EditorGUILayout.PropertyField(Stackable, new GUIContent("Stackable", "Is the item stackable? NO for Weapons or Armour"));
            //            }
            //            EditorGUILayout.PropertyField(Equippable, new GUIContent("Equippable", "Is the item equippable?"));
            //            EditorGUILayout.PropertyField(Usable, new GUIContent("Usable", "Is the item usable?"));
            //            EditorGUILayout.PropertyField(Droppable, new GUIContent("Droppable", "Is the item droppable?"));
            //            EditorGUILayout.PropertyField(Discardable, new GUIContent("Discardable", "Is the item discardable/removable?"));
            //            break;
            //        case 5: //Key Item
            //            using (new EditorGUI.DisabledGroupScope(true))
            //            {
            //                EditorGUILayout.PropertyField(Stackable, new GUIContent("Stackable", "Is the item stackable? NO for Weapons or Armour"));
            //                if (Stackable.boolValue == true)
            //                {
            //                    EditorGUILayout.PropertyField(maxStacks, new GUIContent("Max Stacks", "The maxmimum number allowed in one stack"));
            //                }
            //                EditorGUILayout.PropertyField(Equippable, new GUIContent("Equippable", "Is the item equippable?"));
            //                EditorGUILayout.PropertyField(Usable, new GUIContent("Usable", "Is the item usable?"));
            //                EditorGUILayout.PropertyField(Droppable, new GUIContent("Droppable", "Is the item droppable?"));
            //                EditorGUILayout.PropertyField(Discardable, new GUIContent("Discardable", "Is the item discardable/removable?"));
            //            }

            //            break;
            //        default:
            //            EditorGUILayout.PropertyField(Stackable, new GUIContent("Stackable", "Is the item stackable? NO for Weapons or Armour"));
            //            if (Stackable.boolValue == true)
            //            {
            //                EditorGUILayout.PropertyField(maxStacks, new GUIContent("Max Stacks", "The maxmimum number allowed in one stack"));
            //            }
            //            EditorGUILayout.PropertyField(Equippable, new GUIContent("Equippable", "Is the item equippable?"));
            //            EditorGUILayout.PropertyField(Usable, new GUIContent("Usable", "Is the item usable?"));
            //            EditorGUILayout.PropertyField(Droppable, new GUIContent("Droppable", "Is the item droppable?"));
            //            EditorGUILayout.PropertyField(Discardable, new GUIContent("Discardable", "Is the item discardable/removable?"));
            //            break;
            //    }
            //}
            #endregion
        }
    }

    private void ShowReactions()
    {

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            MalbersEditor.DrawDescription("Inventory Button Reactions");
            EditorGUI.indentLevel++;
            if (Usable.boolValue == true)
            {
                EditorGUILayout.PropertyField(UseReaction);
            }
            if (Equippable.boolValue == true)
            {
                EditorGUILayout.PropertyField(EquipReaction);
                EditorGUILayout.PropertyField(UnequipReaction);
            }
            if (Droppable.boolValue == true)
            {
                EditorGUILayout.PropertyField(DropReaction);
            }
            if (Discardable.boolValue == true)
            {
                EditorGUILayout.PropertyField(RemoveReaction);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void ShowEvents()
    {
        // using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            MalbersEditor.DrawDescription("Item Events");

            if (Usable.boolValue == true)
            {
                EditorGUILayout.PropertyField(OnItemUsed);
            }
            if (Droppable.boolValue == true)
            {
                EditorGUILayout.PropertyField(OnItemDropped);
            }
            if (Discardable.boolValue == true)
            {
                EditorGUILayout.PropertyField(OnItemRemoved);
            }
            if (Equippable.boolValue == true)
            {
                EditorGUILayout.PropertyField(OnItemEquipped);
                EditorGUILayout.PropertyField(OnItemUnEquipped);
            }
        }
    }
}
#endif