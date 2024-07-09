using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICProtocol
{
    const int INCONNECT = 2222;
    const int BONENUMBER = 6;
    int HEADSIZE;

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

    struct stHeader
    {
        int nID;
        int nSize;
        int nType;
        int nCheckSum;

        stHeader(int nid, int nsize, int ntype, int nchecksum)
        {
            this.nID = nid;
            this.nSize = nsize;
            this.nType = ntype;
            this.nCheckSum = nchecksum;
        }

        void SetHeader(int id, int len)
        {
            this.nType = INCONNECT;
            this.nID = id;
            this.nSize = len;
            this.nCheckSum = nType + nID + nSize;
        }
    }
}
