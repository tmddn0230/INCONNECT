#pragma once
#pragma pack(1)

#include "ICCore.h"

#define INCONNECT 2222
#define BONENUMBER 6
// 


// 전송할 모션 데이터 정보를 담기위한 구조체 



enum enBone
{
	HEAD_BONE = 0,
	SPINE_BONE,
	HIPBONE,
	FOOT_R,
	FOOT_L,
	HAND_R,
	HAND_L
};

enum enProtocol
{
	PROTOCOL_START = 0,
	                  prConnectAck,
	prLoginReq,      prLoginAck, 
	prBoneData,
	prENTPORTAL,     

    PROTOCOL_END
};

typedef struct Bone_Data
{
	uint16 BoneType;

	// float = 4byte
	float PosX;
	float PosY;
	float PosZ;
	float RotX;
	float RotY;
	float RotZ;
	float RotW;

}Bone_Data;

struct ROKOKO_DATA
{
	// ROKOKO STUDIO 에서 값을 어떻게 넘겨주는 지 봐야함
	// PLUGIN 에서 BONE 이나 애니메이션 어떤 값을 보내주는가 

	Bone_Data datas[BONENUMBER];
};

struct stHeader
{
	uint16 nID;
	uint16 nSize;
	uint16 nType;
	uint16 nCheckSum;

	stHeader()
	{
		nID = nSize = nType = nCheckSum = 0;
	};
	void SetHeader(int id, int len)
	{
		nType = INCONNECT;
		nID = id;
		nSize = len;
		nCheckSum = nType + nID + nSize;
	};
};

#define HEADSIZE sizeof( stHeader )

#pragma pack()
