using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations;
using MalbersAnimations.HAP;
using MalbersAnimations.Weapons;
using MalbersAnimations.Utilities;
using UMA;
using UMA.CharacterSystem;

[RequireComponent(typeof(DynamicCharacterAvatar))]
public class UMARider : MonoBehaviour
{
    private DynamicCharacterAvatar characterAvatar;

    public GameObject backHolderWeapon;
    public GameObject rightHolderWeapon;
    public GameObject leftHolderWeapon;

    public Vector3 equipPointRightOffset = new Vector3(0.00980000012f, 0.0573000014f, -0.00779999979f);
    public Vector3 equipPointRightRotation = new Vector3(-65.5419998f, -65.0139999f, 60.3079987f);

    public Vector3 equipPointLeftOffset = new Vector3(0.0299999993f, 0.0110999998f, -0.0274999999f);
    public Vector3 equipPointLeftRotation = new Vector3(-105.530998f, -44.1660194f, 43.1020088f);

    public Vector3 holderBackOffset = new Vector3(-0.0338084996f, -0.046600949f, -0.0858304873f);
    public Vector3 holderBackRotation = new Vector3(120.706001f, 115.587997f, 117.374001f);

    public Vector3 holderRightOffset = new Vector3(0.137999997f, -0.0719999969f, 0.125f);
    public Vector3 holderRightRotation = new Vector3(10.9829998f, -129.022995f, 4.42299986f);

    public Vector3 holderLeftOffset = new Vector3(0.141000003f, 0.103f, 0.0850000009f);
    public Vector3 holderLeftRotation = new Vector3(-10.3769999f, -121.528f, -17.7639999f);

    void Start () {
        characterAvatar = GetComponent<DynamicCharacterAvatar>();
    }

    void Update () {
        
    }

    private void SetCharacterRecipe(string slotToChange, UMATextRecipe recipe)
    {
        if (recipe != null)
        {
            //Debug.Log("Set " + slotToChange + " to " + recipe.DisplayValue);
            characterAvatar.SetSlot(recipe);
        }
        else
        {
            //Debug.Log("Clear " + slotToChange);
            characterAvatar.ClearSlot(slotToChange);
            characterAvatar.ReapplyWardrobeCollections();
        }

        characterAvatar.BuildCharacter(true);
    }

    // should be called on UMA created event
    public void OnCharacterCreated () {

        MWeaponManager weaponManager = gameObject.GetComponent<MWeaponManager>();
        MInventory inventory = gameObject.GetComponent<MInventory>();
        Animator Anim = gameObject.GetComponent<Animator>();
        IAim aim = gameObject.GetComponent<IAim>();
        MalbersAnimations.Controller.MAnimal mAnimal = gameObject.GetComponent<MalbersAnimations.Controller.MAnimal>();
        MRider rider = gameObject.GetComponent<MRider>();
        
        Transform head = transform.Find("Root/Global/Position/Hips/LowerBack/Spine/Spine1/Neck/Head");
        Transform rightHand = transform.Find("Root/Global/Position/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand");
        Transform leftHand = transform.Find("Root/Global/Position/Hips/LowerBack/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        Transform holderBackTransform = transform.Find("Root/Global/Position/Hips/LowerBack/Spine/Spine1");
        Transform holderRightTransform = transform.Find("Root/Global/Position/Hips/RightUpLeg");
        Transform holderLeftTransform = transform.Find("Root/Global/Position/Hips/LeftUpLeg");

        GameObject rightHandEquipPoint = new GameObject("RightHandPoint");
        rightHandEquipPoint.transform.parent = rightHand;
        rightHandEquipPoint.transform.position = rightHand.position;
        rightHandEquipPoint.transform.rotation = rightHand.rotation;
        rightHandEquipPoint.transform.localPosition = equipPointRightOffset;
        rightHandEquipPoint.transform.localRotation = Quaternion.Euler(equipPointRightRotation);
        rightHandEquipPoint.transform.localScale = new Vector3(1f,1f,1f);

        GameObject leftHandPoint = new GameObject("LeftHandPoint");
        leftHandPoint.transform.parent = leftHand;
        leftHandPoint.transform.position = leftHand.position;
        leftHandPoint.transform.rotation = leftHand.rotation;
        leftHandPoint.transform.localPosition = equipPointLeftOffset;
        leftHandPoint.transform.localRotation = Quaternion.Euler(equipPointLeftRotation);
        leftHandPoint.transform.localScale = new Vector3(1f,1f,1f);

        GameObject holderBack = new GameObject("HolderBack");
        holderBack.transform.parent = holderBackTransform;
        holderBack.transform.position = holderBackTransform.position;
        holderBack.transform.rotation = holderBackTransform.rotation;
        holderBack.transform.localPosition = holderBackOffset;
        holderBack.transform.localRotation = Quaternion.Euler(holderBackRotation);
        holderBack.transform.localScale = new Vector3(1f,1f,1f);

        GameObject holderRight = new GameObject("HolderRight");
        holderRight.transform.parent = holderRightTransform;
        holderRight.transform.position = holderRightTransform.position;
        holderRight.transform.rotation = holderRightTransform.rotation;
        holderRight.transform.localPosition = holderRightOffset;
        holderRight.transform.localRotation = Quaternion.Euler(holderRightRotation);
        holderRight.transform.localScale = new Vector3(1f,1f,1f);

        GameObject holderLeft = new GameObject("HolderLeft");
        holderLeft.transform.parent = holderLeftTransform;
        holderLeft.transform.position = holderLeftTransform.position;
        holderLeft.transform.rotation = holderLeftTransform.rotation;
        holderLeft.transform.localPosition = holderLeftOffset;
        holderLeft.transform.localRotation = Quaternion.Euler(holderLeftRotation);
        holderLeft.transform.localScale = new Vector3(1f,1f,1f);

        // this works with the Rider's holders, which means that the weapons should already be instantiated
        if (backHolderWeapon)
        {
            GameObject backWeapon = Instantiate(backHolderWeapon, holderBack.transform.position, holderBack.transform.rotation);
            if (backWeapon)
            {
                backWeapon.transform.parent = holderBack.transform;
                inventory.Inventory.Add(backWeapon);
            }
        }

        if (rightHolderWeapon)
        {
            GameObject rightWeapon = Instantiate(rightHolderWeapon, holderRight.transform.position, holderRight.transform.rotation);
            if (rightWeapon)
            {
                rightWeapon.transform.parent = holderRight.transform;
                inventory.Inventory.Add(rightWeapon);
            }
        }

        if (leftHolderWeapon)
        {
            GameObject leftWeapon = Instantiate(leftHolderWeapon, holderLeft.transform.position, holderLeft.transform.rotation);
            if (leftWeapon)
            {
                leftWeapon.transform.parent = holderLeft.transform;
                inventory.Inventory.Add(leftWeapon);
            }
        }

        weaponManager.holsters[0].Slots.Add(holderLeft.transform);
        weaponManager.holsters[0].Slots.Add(holderBack.transform); // TODO: need an additional back holster for left
        weaponManager.holsters[1].Slots.Add(holderBack.transform);
        weaponManager.holsters[2].Slots.Add(holderRight.transform);

        //rider.RightHandEquipPoint = rightHandEquipPoint.transform;
        //rider.LeftHandEquipPoint = leftHandPoint.transform;
        //rider.HolderBack = holderBack.transform;
        //rider.HolderRight = holderRight.transform;
        //rider.HolderLeft = holderLeft.transform;

        // we had to change set accessors for these on the RiderCombat class in MRider.cs
        //rider.RiderRoot = transform.root;
        rider.Chest = Anim.GetBoneTransform(HumanBodyBones.Chest);                   //Get the Rider Head transform
        rider.Spine = Anim.GetBoneTransform(HumanBodyBones.Spine);                     //Get the Rider Head transform
        //--------------
        //rider.Head = Anim.GetBoneTransform(HumanBodyBones.Head);                     //Get the Rider Head transform
        rider.RightHand = Anim.GetBoneTransform(HumanBodyBones.RightHand);           //Get the Rider Right Hand transform
        rider.LeftHand = Anim.GetBoneTransform(HumanBodyBones.LeftHand);             //Get the Rider Left  Hand transform
        //rider.RightShoulder = Anim.GetBoneTransform(HumanBodyBones.RightUpperArm);   //Get the Rider Right Shoulder transform
        //rider.LeftShoulder = Anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);

        //weaponManager.RightShoulder = Anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        //weaponManager.LeftShoulder = Anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        weaponManager.RightHand = Anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponManager.LeftHand = Anim.GetBoneTransform(HumanBodyBones.LeftHand);
        //weaponManager.Head = Anim.GetBoneTransform(HumanBodyBones.Head);
        //weaponManager.Chest = Anim.GetBoneTransform(HumanBodyBones.Chest);

        weaponManager.LeftHandEquipPoint = leftHandPoint.transform;
        weaponManager.RightHandEquipPoint = rightHandEquipPoint.transform;

        aim.AimOrigin = head;

        mAnimal.ResetController();
    }
}
