using MalbersAnimations;
using MalbersAnimations.InventorySystem;
using MalbersAnimations.Weapons;
using UMA;
using UnityEngine;

[System.Serializable]
[AddTypeMenu("UMA/Equip Clothing Item")]
public class EquipClothingItemReaction : InventoryMasterReaction
{
    protected override bool _TryReact(Component component)
    {
        //Get Main Character and Weapon Manager Reference.
        InventoryMaster invMaster = component as InventoryMaster;
        if (invMaster == null) return false;

        // get item as UMAWardrobeItem
        UMAWardrobeItem wardrobeItem = invMaster.currentSelectedSlot.item as UMAWardrobeItem;
        // set the current inventory slot
        wardrobeItem.inventorySlot = invMaster.currentSelectedSlot;

        //Debug.Log("Equipped: <color=yellow>" + invMaster.currentSelectedSlot.item.name + "</color>");

        UMARider umaRider = invMaster.character.Value.GetComponent<UMARider>();
        // let umarider handle the equipping
        umaRider.EquipUMAWardrobeItem(wardrobeItem);

        if (SaveLoadSystem.instance == null || !SaveLoadSystem.instance.isLoadingData)
        {
            invMaster.currentSelectedSlot.draggable.FocusOnItem();
            //OnItemEquipped.Invoke(null);
            invMaster.currentSelectedSlot.item.OnItemEquipped.Invoke(null); //Item Event
            invMaster.currentSelectedSlot.inventory.OnItemEquipped.Invoke(null); //Inventory Event
        }

        return true;
    }
}