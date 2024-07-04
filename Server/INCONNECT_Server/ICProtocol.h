#pragma once
#pragma pack(1)

#include "ICCore.h"

#define INCONNECT 2222

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
