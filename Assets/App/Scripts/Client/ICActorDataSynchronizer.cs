using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rokoko.Inputs;

public class ICActorDataSynchronizer : MonoBehaviour
{
    public Actor m_actor;
    public CoreBoneData m_coreBoneData;

    // Update is called once per frame
    void Update()
    {
        if (m_actor == null) return;

        m_coreBoneData.hipPosition = GetPositionArray(HumanBodyBones.Hips);
        m_coreBoneData.hipRotation = GetRotationArray(HumanBodyBones.Hips);
        m_coreBoneData.spinePosition = GetPositionArray(HumanBodyBones.Spine);
        m_coreBoneData.spineRotation = GetRotationArray(HumanBodyBones.Spine);
        m_coreBoneData.chestPosition = GetPositionArray(HumanBodyBones.Chest);
        m_coreBoneData.chestRotation = GetRotationArray(HumanBodyBones.Chest);
        m_coreBoneData.neckPosition = GetPositionArray(HumanBodyBones.Neck);
        m_coreBoneData.neckRotation = GetRotationArray(HumanBodyBones.Neck);
        m_coreBoneData.headPosition = GetPositionArray(HumanBodyBones.Head);
        m_coreBoneData.headRotation = GetRotationArray(HumanBodyBones.Head);
        m_coreBoneData.leftFootPosition = GetPositionArray(HumanBodyBones.LeftFoot);
        m_coreBoneData.leftFootRotation = GetRotationArray(HumanBodyBones.LeftFoot);
        m_coreBoneData.rightFootPosition = GetPositionArray(HumanBodyBones.RightFoot);
        m_coreBoneData.rightFootRotation = GetRotationArray(HumanBodyBones.RightFoot);

        // 팔 추가
        m_coreBoneData.leftUpperArmPosition = GetPositionArray(HumanBodyBones.LeftUpperArm);
        m_coreBoneData.leftUpperArmRotation = GetRotationArray(HumanBodyBones.LeftUpperArm);
        m_coreBoneData.leftLowerArmPosition = GetPositionArray(HumanBodyBones.LeftLowerArm);
        m_coreBoneData.leftLowerArmRotation = GetRotationArray(HumanBodyBones.LeftLowerArm);
        m_coreBoneData.leftHandPosition = GetPositionArray(HumanBodyBones.LeftHand);
        m_coreBoneData.leftHandRotation = GetRotationArray(HumanBodyBones.LeftHand);
        m_coreBoneData.rightUpperArmPosition = GetPositionArray(HumanBodyBones.RightUpperArm);
        m_coreBoneData.rightUpperArmRotation = GetRotationArray(HumanBodyBones.RightUpperArm);
        m_coreBoneData.rightLowerArmPosition = GetPositionArray(HumanBodyBones.RightLowerArm);
        m_coreBoneData.rightLowerArmRotation = GetRotationArray(HumanBodyBones.RightLowerArm);
        m_coreBoneData.rightHandPosition = GetPositionArray(HumanBodyBones.RightHand);
        m_coreBoneData.rightHandRotation = GetRotationArray(HumanBodyBones.RightHand);
        // 실시간으로 데이터를 업데이트합니다.
        CharacterAnim.Instance?.UpdateCharacter(m_coreBoneData);
    }

    private float[] GetPositionArray(HumanBodyBones bone)
    {
        Transform boneTransform = m_actor.GetBone(bone);
        if (boneTransform == null) return new float[3];

        Vector3 position = boneTransform.position;
        return new float[] { position.x, position.y, position.z };
    }

    private float[] GetRotationArray(HumanBodyBones bone)
    {
        Transform boneTransform = m_actor.GetBone(bone);
        if (boneTransform == null) return new float[4];

        Quaternion rotation = boneTransform.rotation;
        return new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
    }
}

public struct CoreBoneData
{
    // Body : 5
    public float[] headPosition;
    public float[] headRotation;
    public float[] neckPosition;
    public float[] neckRotation;
    public float[] chestPosition;
    public float[] chestRotation;
    public float[] spinePosition;
    public float[] spineRotation;
    public float[] hipPosition;
    public float[] hipRotation;


    // Hand : 6
    public float[] leftUpperArmPosition;
    public float[] leftUpperArmRotation;
    public float[] leftLowerArmPosition;
    public float[] leftLowerArmRotation;
    public float[] leftHandPosition;
    public float[] leftHandRotation;
    public float[] rightUpperArmPosition;
    public float[] rightUpperArmRotation;
    public float[] rightLowerArmPosition;
    public float[] rightLowerArmRotation;
    public float[] rightHandPosition;
    public float[] rightHandRotation;

    // Leg : 2
    public float[] leftFootPosition;
    public float[] leftFootRotation;
    public float[] rightFootPosition;
    public float[] rightFootRotation;
}


