#pragma once
#pragma pack(1)

#include "Core.h"
#include "ICDefine.h"

enum enErrorCode
{
	ERROR_NODEVICE = -43,
	ERROR_NOTRAINEE = -42,
	ERROR_ALREADY_RESULT = -41, // There is Result of Training already.
	ERROR_NOTRAININGNUMBER = -40,
	ERROR_DUP_ROLE = -7, // Duplicate Role ID
	ERROR_DUP_TRAINEE = -6, // Duplicate Trainee (UID)
	ERROR_DUO_NUMBER = -5, // Duplicate Army Number 
	ERROR_PASSWORD = -4,
	ERROR_ACCOUNT = -2,   // No Account
	ERROR_DUP_ACCOUNT = -1,
	FAIL = 0,
	SUCCESS = 1
};

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
	prLoginReq, prLoginAck,
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
		nID = id;
		nSize = len;
		nType = INCONNECT;
		nCheckSum = nType + nID + nSize;
	};
};

#define HEADSIZE sizeof( stHeader )

#pragma pack()

