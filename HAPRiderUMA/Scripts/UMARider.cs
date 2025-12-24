using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations;
using MalbersAnimations.HAP;
using MalbersAnimations.Weapons;
using MalbersAnimations.Utilities;
using UMA;
using UMA.CharacterSystem;
using System;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(DynamicCharacterAvatar))]
public class UMARider : MonoBehaviour
{
    private DynamicCharacterAvatar characterAvatar;
    private MWeaponManager weaponManager;
    private MInventory inventory;
    private Animator animator;
    private IAim aim;
    private LookAt lookAt;
    private MalbersAnimations.Controller.MAnimal mAnimal;
    private MRider rider;
    public HumanoidParent[] humanoidParents;

    private Dictionary<string, Vector3> humanoidParentsPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, Quaternion> humanoidParentsRotations = new Dictionary<string, Quaternion>();

    private Dictionary<string, UMAWardrobeItem> equippedWardrobeItems = new Dictionary<string, UMAWardrobeItem>();

    private Transform root;
    private Transform head;
    private Transform neck;
    private Transform rightHand;
    private Transform leftHand;

    // equip points should become humanoid parents
    public GameObject rightHandEquipPoint;
    public GameObject leftHandEquipPoint;

    /*public GameObject backHolderWeapon;
    public GameObject rightHolderWeapon;
    public GameObject leftHolderWeapon;*/

    public Vector3 equipPointRightOffset = new Vector3(0.00980000012f, 0.0573000014f, -0.00779999979f);
    public Vector3 equipPointRightRotation = new Vector3(-65.5419998f, -65.0139999f, 60.3079987f);

    public Vector3 equipPointLeftOffset = new Vector3(0.0299999993f, 0.0110999998f, -0.0274999999f);
    public Vector3 equipPointLeftRotation = new Vector3(-105.530998f, -44.1660194f, 43.1020088f);

    private void Awake()
    {
        DynamicCharacterAvatar dca = GetComponent<DynamicCharacterAvatar>();
        dca.CharacterCreated.AddListener(delegate { OnCharacterCreated(); });
        dca.CharacterUpdated.AddListener(delegate { OnCharacterUpdated(); });
    }

    void Start()
    {
        characterAvatar = GetComponent<DynamicCharacterAvatar>();
        weaponManager = gameObject.GetComponent<MWeaponManager>();
        inventory = gameObject.GetComponent<MInventory>();
        animator = gameObject.GetComponent<Animator>();
        aim = gameObject.GetComponent<IAim>();
        lookAt = gameObject.GetComponent<LookAt>();
        mAnimal = gameObject.GetComponent<MalbersAnimations.Controller.MAnimal>();
        rider = gameObject.GetComponent<MRider>();
        humanoidParents = gameObject.GetComponentsInChildren<HumanoidParent>();

        foreach (HumanoidParent humanoidParent in humanoidParents)
        {
            humanoidParentsPositions.Add(humanoidParent.name, humanoidParent.transform.localPosition);
            humanoidParentsRotations.Add(humanoidParent.name, humanoidParent.transform.localRotation);
        }
    }

    private void FindCharacterTransforms()
    {
        root = transform.Find("Root");
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
    }

    public bool EquipUMAWardrobeItem(UMAWardrobeItem wardrobeItem)
    {
        if(!wardrobeItem.textRecipe.compatibleRaces.Contains(characterAvatar.activeRace.name)) { return false; }

        Debug.Log("Equip: "+wardrobeItem.textRecipe.name+" "+wardrobeItem.textRecipe.wardrobeSlot);
        if(equippedWardrobeItems.ContainsKey(wardrobeItem.textRecipe.wardrobeSlot))
        {
            Debug.Log("Unequip Previous item: <color=yellow>" + equippedWardrobeItems[wardrobeItem.textRecipe.wardrobeSlot].name + "</color>");

            UnequipUMAWardrobeItem(equippedWardrobeItems[wardrobeItem.textRecipe.wardrobeSlot], true);
        }

        //Debug.Log("Equip new item: <color=yellow>" + wardrobeItem.name + "</color>");
        equippedWardrobeItems.Add(wardrobeItem.textRecipe.wardrobeSlot, wardrobeItem);
        wardrobeItem.inventorySlot.equippedSlot = true;
        wardrobeItem.inventorySlot.EquippedText.gameObject.SetActive(true);

        //Debug.Log("UMA Equip: <color=yellow>" + wardrobeItem.name + "</color>");
        DynamicCharacterAvatar.WardrobeRecipeListItem wardrobeRecipeListItem = new DynamicCharacterAvatar.WardrobeRecipeListItem();
        wardrobeRecipeListItem._recipe = wardrobeItem.textRecipe;
        wardrobeRecipeListItem._recipeName = wardrobeItem.textRecipeName;
        characterAvatar.preloadWardrobeRecipes.recipes.Add(wardrobeRecipeListItem);
        characterAvatar.WardrobeRecipes.Add(wardrobeItem.textRecipeName, wardrobeItem.textRecipe);
        characterAvatar.SetSlot(wardrobeItem.textRecipe);

        characterAvatar.BuildCharacter(true);

        return true;

    }

    public void UnequipUMAWardrobeItem(UMAWardrobeItem wardrobeItem, bool noRebuild = false)
    {
        Debug.Log("Unequip: " + wardrobeItem.textRecipe.name + " " + wardrobeItem.textRecipe.wardrobeSlot);
        if (equippedWardrobeItems.ContainsKey(wardrobeItem.textRecipe.wardrobeSlot))
        {
            equippedWardrobeItems.Remove(wardrobeItem.textRecipe.wardrobeSlot);
        }

        wardrobeItem.inventorySlot.inventory.OnItemUnEquipped.Invoke(null); //Inventory Event

        wardrobeItem.inventorySlot.equippedSlot = false;
        wardrobeItem.inventorySlot.EquippedText.gameObject.SetActive(false);

        //Debug.Log("UMA Unequipped: <color=yellow>" + wardrobeItem.textRecipe.wardrobeSlot + "</color>");
        DynamicCharacterAvatar.WardrobeRecipeListItem wardrobeRecipeListItem = characterAvatar.preloadWardrobeRecipes.recipes.Where(x => x._recipeName == wardrobeItem.textRecipeName).First<DynamicCharacterAvatar.WardrobeRecipeListItem>();
        characterAvatar.preloadWardrobeRecipes.recipes.Remove(wardrobeRecipeListItem);
        characterAvatar.WardrobeRecipes.Remove(wardrobeItem.textRecipeName);
        characterAvatar.ClearSlot(wardrobeItem.textRecipe.wardrobeSlot);

        if (!noRebuild)
        {
            characterAvatar.BuildCharacter(true);
        }
    }

    /// <summary>
    /// <para>Unparent our created equip points.</para>
    /// <para>Used to catch them before they are lost at rebuild of character.</para>
    /// </summary>
    private void FireBeforeCharacterUpdate()
    {
        Debug.Log("FireBeforeCharacterUpdate");
        humanoidParents = gameObject.GetComponentsInChildren<HumanoidParent>();
        foreach (HumanoidParent humanoidParent in humanoidParents)
        {
            humanoidParent.transform.SetParent(null, true);
        }

        rightHandEquipPoint.transform.SetParent(null, true);
        leftHandEquipPoint.transform.SetParent(null, true);
    }

    /// <summary>
    /// Updates the transform of each humanoid parent to match the corresponding bone transform in the animator.
    /// </summary>
    /// <remarks>This method iterates over all humanoid parents and adjusts their transform properties such as
    /// position, rotation, and scale to align with the animator's bone transforms. It also applies any specified
    /// position and rotation offsets. Additionally, it sets the parent for right and left hand equip points.</remarks>
    private void FireAfterCharacterUpdate()
    {
        foreach (HumanoidParent humanoidParent in humanoidParents)
        {
            if (animator != null)
            {
                var boneParent = animator.GetBoneTransform(humanoidParent.parent);
                // this is almost the same as calling HumanoidParent.Align, but that's a private function and I'm setting scale as well.
                if (boneParent != null)
                {
                    /*humanoidParent.transform.position = transform.position+humanoidParentsPositions[humanoidParent.name];
                    humanoidParent.transform.rotation = humanoidParentsRotations[humanoidParent.name];*/

                    humanoidParent.transform.parent = boneParent;

                    if (humanoidParent.LocalPos.Value) humanoidParent.transform.localPosition = Vector3.zero;
                    if (humanoidParent.LocalRot.Value) humanoidParent.transform.localRotation = Quaternion.identity;
                    
                    humanoidParent.transform.localScale = boneParent.transform.localScale;
                    humanoidParent.transform.localPosition += humanoidParent.PosOffset;
                    humanoidParent.transform.localRotation *= Quaternion.Euler(humanoidParent.RotOffset);
                }
            }
        }

        SetRightHandEquipPointParent();

        SetLeftHandEquipPointParent();
    }

    /// <summary>
    /// Handles updates to the character's state and configuration after a UMA update event. Should be called on UMA updated event.
    /// </summary>
    /// <remarks>This method should be called in response to a UMA character update event. It ensures that all
    /// character-related transforms and components are correctly set up and ready for use.</remarks>
    public void OnCharacterUpdated()
    {
        FindCharacterTransforms();

        FireAfterCharacterUpdate();

        SetWeaponManagerTransforms();

        SetRiderTransforms();

        SetLookAtBones();

        mAnimal.ResetController();
    }

    /// <summary>
    /// Initializes character components and sets up equipment points after a character is created. Should be called on UMA created event.
    /// </summary>
    /// <remarks>This method should be called in response to the UMA character creation event. It sets up the
    /// necessary transforms for character equipment and initializes the character's weapon management and rider
    /// components. The method also configures the character's look-at bones and resets the character
    /// controller.</remarks>
    public void OnCharacterCreated ()
    {
        FindCharacterTransforms();

        rightHandEquipPoint = new GameObject("RightHandPoint");
        SetRightHandEquipPointParent();

        leftHandEquipPoint = new GameObject("LeftHandPoint");
        SetLeftHandEquipPointParent();

        SetWeaponManagerTransforms();

        SetRiderTransforms();

        SetLookAtBones();

        FireAfterCharacterUpdate();

        // this works with the Rider's holders, which expects that the weapons should already be instantiated
        /*if (backHolderWeapon)
        {
            GameObject backWeapon = Instantiate(backHolderWeapon, weaponManager.holsters[1].Slots[0].position, weaponManager.holsters[1].Slots[0].rotation);
            if (backWeapon)
            {
                backWeapon.transform.parent = weaponManager.holsters[1].Slots[0];
                inventory.Inventory.Add(backWeapon);
            }
        }

        if (rightHolderWeapon)
        {
            GameObject rightWeapon = Instantiate(rightHolderWeapon, weaponManager.holsters[2].Slots[0].position, weaponManager.holsters[2].Slots[0].rotation);
            if (rightWeapon)
            {
                rightWeapon.transform.parent = weaponManager.holsters[2].Slots[0];
                inventory.Inventory.Add(rightWeapon);
            }
        }

        if (leftHolderWeapon)
        {
            GameObject leftWeapon = Instantiate(leftHolderWeapon, weaponManager.holsters[0].Slots[0].position, weaponManager.holsters[0].Slots[0].rotation);
            if (leftWeapon)
            {
                leftWeapon.transform.parent = weaponManager.holsters[0].Slots[0];
                inventory.Inventory.Add(leftWeapon);
            }
        }*/

        mAnimal.Rotator = root;
        mAnimal.RootBone = root;

        mAnimal.ResetController();
    }

    private void SetRiderTransforms()
    {
        if (rider)
        {
            //rider.RightHandEquipPoint = rightHandEquipPoint.transform;
            //rider.LeftHandEquipPoint = leftHandPoint.transform;
            //rider.HolderBack = holderBack.transform;
            //rider.HolderRight = holderRight.transform;
            //rider.HolderLeft = holderLeft.transform;

            // we had to change set accessors for these on the RiderCombat class in MRider.cs
            //rider.RiderRoot = transform.root;
            rider.Chest = animator.GetBoneTransform(HumanBodyBones.Chest);                   //Get the Rider Head transform
            rider.Spine = animator.GetBoneTransform(HumanBodyBones.Spine);                     //Get the Rider Head transform
            //--------------
            //rider.Head = Anim.GetBoneTransform(HumanBodyBones.Head);                     //Get the Rider Head transform
            rider.RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);           //Get the Rider Right Hand transform
            rider.LeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);             //Get the Rider Left  Hand transform
            //rider.RightShoulder = Anim.GetBoneTransform(HumanBodyBones.RightUpperArm);   //Get the Rider Right Shoulder transform
            //rider.LeftShoulder = Anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        }
    }

    private void SetWeaponManagerTransforms()
    {
        if (weaponManager)
        {
            //weaponManager.RightShoulder = Anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            //weaponManager.LeftShoulder = Anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            weaponManager.RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            weaponManager.LeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            //weaponManager.Head = Anim.GetBoneTransform(HumanBodyBones.Head);
            //weaponManager.Chest = Anim.GetBoneTransform(HumanBodyBones.Chest);

            weaponManager.LeftHandEquipPoint = leftHandEquipPoint.transform;
            weaponManager.RightHandEquipPoint = rightHandEquipPoint.transform;
        }
    }

    void SetRightHandEquipPointParent()
    {
        rightHandEquipPoint.transform.parent = rightHand;
        rightHandEquipPoint.transform.position = rightHand.position;
        rightHandEquipPoint.transform.rotation = rightHand.rotation;
        rightHandEquipPoint.transform.localPosition = equipPointRightOffset;
        rightHandEquipPoint.transform.localRotation = Quaternion.Euler(equipPointRightRotation);
        rightHandEquipPoint.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void SetLeftHandEquipPointParent()
    {
        leftHandEquipPoint.transform.parent = leftHand;
        leftHandEquipPoint.transform.position = leftHand.position;
        leftHandEquipPoint.transform.rotation = leftHand.rotation;
        leftHandEquipPoint.transform.localPosition = equipPointLeftOffset;
        leftHandEquipPoint.transform.localRotation = Quaternion.Euler(equipPointLeftRotation);
        leftHandEquipPoint.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void SetLookAtBones()
    {
        if (lookAt != null)
        {
            lookAt.Bones[0].bone = neck;
            lookAt.Bones[1].bone = head;
            lookAt.SetEnable(true);
        }
    }
}