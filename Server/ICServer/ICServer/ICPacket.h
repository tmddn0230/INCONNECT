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
	stLoginReq()
	{
		SetHeader(prLoginReq, sizeof(stLoginReq));
	};
};

struct stLoginAck : public stHeader
{


	stLoginAck()
	{
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

struct stEntPortal : public stHeader
{
	stEntPortal()
	{
		SetHeader(prENTPORTAL, sizeof(stEntPortal));
	};
};


struct stTestPacket
{
	int UID;
	float v;
	float v1;
	float v2;
	float v3;
	float v4;
	float v5;
	float v6;
	float v7;
	float v8;
	float q;
	float q1;
	float q2;
	float q3;
	float q4;
	float q5;
	float q6;
	float q7;
	float q8;
	float q9;
	float q10;
	float q11;
	float q12;
	float q13;
	float q14;
	float q15;


};
