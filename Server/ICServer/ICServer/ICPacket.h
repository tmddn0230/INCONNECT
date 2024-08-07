#pragma once

#include "Core.h"
#include "ICProtocol.h"
#include "ICStruct.h"

#pragma pack(1)
struct stConnectAck : public stHeader
{
	// Connect
	int32 Index;
	stConnectAck()
	{
		Index = 0;

		SetHeader(prConnectAck, sizeof(stConnectAck));
	};
};

struct stLoginReq : public stHeader
{
	int32 UID;
	int32 Result;
	stLoginReq()
	{
		UID = 0;
		Result = 0;
		SetHeader(prLoginReq, sizeof(stLoginReq));
	};
};

struct stLoginAck : public stHeader
{
	int32 UID;
	int32 Result;
	stLoginAck()
	{
		UID = 0;
		Result = 0;
		SetHeader(prLoginAck, sizeof(stLoginAck));
	};
};

struct stBoneData : public stHeader
{
	uint32 UID;
	float v[3][13];
	float q[4][13];

	stBoneData()
	{
		UID = 0;
		memset(v, 0, sizeof(v));
		memset(q, 0, sizeof(q));
		SetHeader(prBoneData, sizeof(stBoneData));
	};
};

struct stTransform : public stHeader
{
	uint32 UID;
	float v[3];
	float q[4];

	stTransform()
	{
		UID = 0;
		memset(v, 0, sizeof(v));
		memset(q, 0, sizeof(q));
		SetHeader(prTransform, sizeof(stTransform));
	};
};


struct stMatchingReq : public stHeader
{
	uint32 UID;

	stMatchingReq()
	{
		UID = 0;
		SetHeader(prMatchingReq, sizeof(stMatchingReq));
	};
};


struct stMatchingAck : public stHeader
{
	uint32 Result;

	stMatchingAck()
	{
		Result = 0;
		SetHeader(prMatchingAck, sizeof(stMatchingAck));
	};
};

struct stFirstAttract : public stHeader
{
	uint32 Score;

	stFirstAttract()
	{
		Score = 0;
		SetHeader(prFirstAttract, sizeof(stFirstAttract));
	};
};

struct stMBTI : public stHeader
{
	uint32 Score;

	stMBTI()
	{
		Score = 0;
		SetHeader(prMBTI, sizeof(stMBTI));
	};
};


struct stSendEmo : public stHeader
{
	uint32 EmoNumber;

	stSendEmo()
	{
		EmoNumber = 0;
		SetHeader(prSendEmo, sizeof(stSendEmo));
	};
};

