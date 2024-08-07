using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rokoko.Inputs;
using Packet;

public class ICActorDataSynchronizer : MonoBehaviour
{
    public Actor m_actor;
    public CoreBoneData m_coreBoneData;
    public ICNetworkManager mNetworkManager;
    public int testUID;

    private void Start()
    {
        mNetworkManager = GameObject.Find("NetworkManager").GetComponent<ICNetworkManager>();
    }

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
        //if (testUID == 1 /*&& Input.GetKeyDown(KeyCode.Alpha5)*/)
        //{
        //    testupdate();
        //    return;
        //}
        //if (testUID == 2 /*&& Input.GetKeyDown(KeyCode.Alpha4)*/)
        //{
        //    testsend();
        //}
        //CharacterAnim.Instance?.UpdateCharacter(m_coreBoneData);
        
    }
    void testsend()
    {
        if(mNetworkManager != null)
        {
            ICPacket_Bone bonepacket = new ICPacket_Bone();
            bonepacket.SetMotionProtocol();
            bonepacket.UID = 2;
            bonepacket.bonedata = m_coreBoneData;

            mNetworkManager.SendPacket_Bone(bonepacket);
        }
    }

    void testupdate()
    {
        if(mNetworkManager != null)
        {
            mNetworkManager.GetReciever().GetDictionValue(1, m_coreBoneData);
            //CharacterAnim.Instance?.UpdateCharacter(m_coreBoneData);
        }
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

    public void Init()
    {
      // Body : 5
      headPosition = new float[3];
      headRotation = new float[4];
      neckPosition = new float[3];
      neckRotation = new float[4];
      chestPosition = new float[3];
      chestRotation = new float[4];
      spinePosition = new float[3];
      spineRotation = new float[4];
      hipPosition = new float[3];
      hipRotation = new float[4];


      // Hand : 6
      leftUpperArmPosition = new float[3];
      leftUpperArmRotation = new float[4];
      leftLowerArmPosition = new float[3];
      leftLowerArmRotation = new float[4];
      leftHandPosition = new float[3];
      leftHandRotation = new float[4];
      rightUpperArmPosition = new float[3];
      rightUpperArmRotation = new float[4];
      rightLowerArmPosition = new float[3];
      rightLowerArmRotation = new float[4];
      rightHandPosition = new float[3];
      rightHandRotation = new float[4];
      
      // Leg : 2
      leftFootPosition = new float[3];
      leftFootRotation = new float[4];
      rightFootPosition = new float[3];
      rightFootRotation = new float[4];
    }
}


