using Rokoko.Core;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{

    public static CharacterAnim Instance;

    public Animator m_animator;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCharacter(CoreBoneData coreBoneData)
    {
        UpdateBone(HumanBodyBones.Hips, coreBoneData.hipPosition, coreBoneData.hipRotation);
        UpdateBone(HumanBodyBones.Spine, coreBoneData.spinePosition, coreBoneData.spineRotation);
        UpdateBone(HumanBodyBones.Chest, coreBoneData.chestPosition, coreBoneData.chestRotation);
        UpdateBone(HumanBodyBones.Neck, coreBoneData.neckPosition, coreBoneData.neckRotation);
        UpdateBone(HumanBodyBones.Head, coreBoneData.headPosition, coreBoneData.headRotation);
        UpdateBone(HumanBodyBones.LeftFoot, coreBoneData.leftFootPosition, coreBoneData.leftFootRotation);
        UpdateBone(HumanBodyBones.RightFoot, coreBoneData.rightFootPosition, coreBoneData.rightFootRotation);

        UpdateBone(HumanBodyBones.LeftUpperArm, coreBoneData.leftUpperArmPosition, coreBoneData.leftUpperArmRotation);
        UpdateBone(HumanBodyBones.LeftLowerArm, coreBoneData.leftLowerArmPosition, coreBoneData.leftLowerArmRotation);
        UpdateBone(HumanBodyBones.LeftHand, coreBoneData.leftHandPosition, coreBoneData.leftHandRotation);
        UpdateBone(HumanBodyBones.RightUpperArm, coreBoneData.rightUpperArmPosition, coreBoneData.rightUpperArmRotation);
        UpdateBone(HumanBodyBones.RightLowerArm, coreBoneData.rightLowerArmPosition, coreBoneData.rightLowerArmRotation);
        UpdateBone(HumanBodyBones.RightHand, coreBoneData.rightHandPosition, coreBoneData.rightHandRotation);

    }

    private void UpdateBone(HumanBodyBones bone, float[] positionArray, float[] rotationArray)
    {
        Transform boneTransform = m_animator.GetBoneTransform(bone);
        if (boneTransform == null) return;

        Vector3 position = new Vector3(positionArray[0], positionArray[1], positionArray[2]);
        Quaternion rotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

        boneTransform.position = position;
        boneTransform.rotation = rotation;
    }
}
