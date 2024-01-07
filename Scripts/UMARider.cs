using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations;
using MalbersAnimations.HAP;
using MalbersAnimations.Weapons;
using MalbersAnimations.Utilities;

public class UMARider : MonoBehaviour
{
    public GameObject backHolderWeapon;
    public GameObject rightHolderWeapon;
    public GameObject leftHolderWeapon;

    public Vector3 equipPointRightOffset = Vector3.zero;
    public Vector3 equipPointRightRotation = Vector3.zero;

    public Vector3 equipPointLeftOffset = Vector3.zero;
    public Vector3 equipPointLeftRotation = Vector3.zero;

    public Vector3 holderBackOffset = Vector3.zero;
    public Vector3 holderBackRotation = Vector3.zero;

    public Vector3 holderRightOffset = Vector3.zero;
    public Vector3 holderRightRotation = Vector3.zero;

    public Vector3 holderLeftOffset = Vector3.zero;
    public Vector3 holderLeftRotation = Vector3.zero;

    void Start () {

    }

    void Update () {
        
    }

    // should be called on UMA created event
    public void UpdateRiderHandBones () {

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
