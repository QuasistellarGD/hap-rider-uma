using MalbersAnimations;
using MalbersAnimations.InventorySystem;
using MalbersAnimations.Weapons;
using UMA;
using UnityEngine;

[System.Serializable]
[AddTypeMenu("UMA/Unequip Clothing Item")]
public class UnequipClothingItemReaction : InventoryMasterReaction
{
    [Tooltip("UMA Clothing recipe to unequip")]

    protected override bool _TryReact(Component component)
    {
        //Get Main Character and Weapon Manager Reference.
        InventoryMaster invMaster = component as InventoryMaster;
        if (invMaster == null) return false;

        if (!invMaster.currentSelectedSlot.equippedSlot)
        {
            return true; // it means we aren't selecting the current equipped slot so therefore we cannot proceed...
        }

        // get item as UMAWardrobeItem
        UMAWardrobeItem wardrobeItem = invMaster.currentSelectedSlot.item as UMAWardrobeItem;
        // set the current inventory slot
        wardrobeItem.inventorySlot = invMaster.currentSelectedSlot;

        UMARider umaRider = invMaster.character.Value.GetComponent<UMARider>();

        umaRider.UnequipUMAWardrobeItem(wardrobeItem);

        if (invMaster.debug) invMaster.Debugging("Unequipping: <color=yellow>" + wardrobeItem.name + "</color>");

        invMaster.currentSelectedSlot.equippedSlot = false;
        invMaster.currentSelectedSlot.EquippedText.gameObject.SetActive(false);
        invMaster.currentSelectedSlot.draggable.FocusOnItem();

        return true;
    }
}