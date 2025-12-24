using MalbersAnimations.InventorySystem;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UMA;
using UMA.CharacterSystem;
using UnityEditor;
using UnityEngine;

public class HAPUMAGenerateWardrobeItemsWindow : EditorWindow
{
    InventoryItem basePrefab;
    List<UMAWardrobeRecipe> wardrobe = null;
    string selectedFolderPath = "No folder selected";

    // Add a menu item to the Unity Editor's menu bar
    [MenuItem("Window/Generate Wardrobe Items")]
    public static void ShowWindow()
    {
        // Get existing open window or create a new one
        HAPUMAGenerateWardrobeItemsWindow window = GetWindow<HAPUMAGenerateWardrobeItemsWindow>("Generate Wardrobe Items");
        window.minSize = new Vector2(200, 150); // Optional: set a minimum window size
        window.Show(); // Display the window
    }

    // This method is called to draw the window's UI
    void OnGUI()
    {
        GUILayout.Label("Select a Base Prefab:", EditorStyles.boldLabel);

        // Use EditorGUILayout.ObjectField to create the selection field
        // Parameters: 
        // 1. Label
        // 2. The current object reference
        // 3. The allowed object type (GameObject)
        // 4. Allow scene objects? (false = only project assets/prefabs)
        basePrefab = (InventoryItem)EditorGUILayout.ObjectField(
            "Base Wardrobe Prefab",
            basePrefab,
            typeof(InventoryItem),
            false); // 'false' restricts to assets only

        GUILayout.Label("Select a folder:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        // Display the selected path
        EditorGUILayout.TextField("Folder Path", selectedFolderPath);

        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            // Open the folder panel
            string path = EditorUtility.OpenFolderPanel("Select Folder", selectedFolderPath == "No folder selected" ? Application.dataPath : selectedFolderPath, "");

            // Check if a path was actually selected (the user didn't cancel)
            if (!string.IsNullOrEmpty(path))
            {
                selectedFolderPath = path;
                // Optional: Focus and highlight the selected folder in the Project window
                SelectFolderInProjectWindow(path);
            }
        }
        EditorGUILayout.EndHorizontal();

        if (basePrefab != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Prefab:", basePrefab.name);

            EditorGUILayout.LabelField("Wardrobe Recipes:", wardrobe.Count+"");

            if (GUILayout.Button("Instantiate Item Prefabs"))
            {

                EditorUtility.DisplayProgressBar("Generate Item Prefabs", "", 0);

                // iterate the global uma library's wardrobe items and generate a UMAWardrobeItem script and releavnt prefab for each wardrobe item
                wardrobe = UMA.UMAAssetIndexer.Instance.GetAllAssets<UMAWardrobeRecipe>();
                for (int i = 0; i < wardrobe.Count; i++)
                {
                    UMAWardrobeRecipe uwr = wardrobe[i];

                    EditorUtility.DisplayProgressBar("Generating Item Prefabs", uwr.AssignedLabel + " "+(i+1)+"/"+wardrobe.Count, ((float)i / (float)wardrobe.Count));

                    string directoryPath = selectedFolderPath + "/" + uwr.wardrobeSlot;
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // 1. Instantiate the source prefab in the current scene
                    InventoryItem inventoryInstance = Instantiate(basePrefab);
                    GameObject instance = inventoryInstance.gameObject;
                    instance.name = uwr.AssignedLabel;
                    instance.GetComponent<UIInteractPassthrough>().item = inventoryInstance;

                    // 3. Save the instantiated GameObject as a new prefab asset
                    bool success;
                    GameObject newPrefabAsset = PrefabUtility.SaveAsPrefabAsset(instance, directoryPath + "/" + instance.name + ".prefab", out success);

                    if (success)
                    {
                        Debug.Log($"Successfully saved new prefab ({instance.name}) asset to: {directoryPath}");

                        // 2. Create a new instance of the ScriptableObject
                        UMAWardrobeItem newWardrobeItem = ScriptableObject.CreateInstance<UMAWardrobeItem>();

                        // 3. Copy serialized data from the source to the new instance
                        // This is crucial for a deep copy of serializable fields
                        EditorUtility.CopySerialized(inventoryInstance.inventoryItem, newWardrobeItem);

                        // Optional: Modify the instance (e.g., change position, add components, etc.)
                        newPrefabAsset.GetComponent<InventoryItem>().inventoryItem = newWardrobeItem;

                        newWardrobeItem.itemName = newWardrobeItem.subtitleText = newWardrobeItem.description = newWardrobeItem.textRecipeName = uwr.AssignedLabel;
                        if (UMAContextBase.Instance.HasRecipe(uwr.AssignedLabel))
                        {
                            newWardrobeItem.textRecipe = UMAContextBase.Instance.GetRecipe(uwr.AssignedLabel, false);
                        }
                        newWardrobeItem.inWorldPrefab = newPrefabAsset;

                        if (uwr.wardrobeRecipeThumbs.Count > 0 && uwr.wardrobeRecipeThumbs[0].thumb != null)
                        {
                            newWardrobeItem.icon = uwr.wardrobeRecipeThumbs[0].thumb;
                            newWardrobeItem.iconPath = uwr.wardrobeRecipeThumbs[0].filename;
                        }

                        AssetDatabase.Refresh();

                        // 5. Save the new instance as a new asset file in the project
                        AssetDatabase.CreateAsset(newWardrobeItem, directoryPath.Replace(Application.dataPath,"Assets") + "/" + instance.name + "_Item.asset");
                        AssetDatabase.SaveAssets();

                        PrefabUtility.SavePrefabAsset(newPrefabAsset);

                    }
                    else
                    {
                        Debug.LogError("Failed to save prefab asset.");
                    }
                    

                    // Optional: Destroy the instance in the scene after saving if it's not needed
                    DestroyImmediate(instance);

                    //Debug.Log(uwr.wardrobeSlot+" "+uwr.name+" "+uwr.recipeType+" "+uwr.label+" "+ uwr.AssignedLabel + " " + uwr.wardrobeRecipeThumbs.Count);
                }

                AssetDatabase.Refresh();

                EditorUtility.ClearProgressBar();

                SelectFolderInProjectWindow(selectedFolderPath);
            }
        }
    }

    // Helper function to select and highlight the folder in the Project window
    private void SelectFolderInProjectWindow(string absolutePath)
    {
        // Convert absolute path to a relative Assets path if necessary
        if (absolutePath.StartsWith(Application.dataPath))
        {
            string relativePath = "Assets" + absolutePath.Substring(Application.dataPath.Length);

            // Load the asset at the path
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);

            if (obj != null)
            {
                Selection.activeObject = obj; // Select the object
                EditorGUIUtility.PingObject(obj); // Highlight it in yellow
            }
        }
    }
}
