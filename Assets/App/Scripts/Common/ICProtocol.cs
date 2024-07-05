using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICProtocol : MonoBehaviour
{
    const int INCONNECT = 2222;
    const int BONENUMBER = 6;

    enum enBone
    {
        HEAD_BONE = 0,
        SPINE_BONE,
        HIPBONE,
        FOOT_R,
        FOOT_L,
        HAND_R,
        HAND_L
    }

    enum enProtocol
    {
        PROTOCOL_START = 0,
        prConnectAck,
        prLoginReq, prLoginAck,
        prBoneData,
        prENTPORTAL,

        PROTOCOL_END
    }

    struct Bone_Data
    {
        ushort BoneType;

        // float = 4byte
        float PosX;
        float PosY;
        float PosZ;
        float RotX;
        float RotY;
        float RotZ;
        float RotW;

        // Construct 
        Bone_Data(ushort type, float px, float py, float pz, 
            float rx, float ry, float rz, float rw)
        {
            this.BoneType = type;
            this.PosX = px; 
            this.PosY = py; 
            this.PosZ = pz; 
            this.RotX = rx;  
            this.RotY = ry;
            this.RotZ = rz;
            this.RotW = rw; 
        }
    }

    struct ROKOKO_DATA
    {
        Bone_Data[] datas;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
