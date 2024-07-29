#pragma once

#include "ICCore.h"
#include "ICProtocol.h"

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
	int UID;
	ROKOKO_DATA rokoko;

	stBoneData()
	{
		UID = 0;
		rokoko = ROKOKO_DATA();
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
