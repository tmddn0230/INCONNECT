#pragma once

#include "Core.h"
#include "ICDefine.h"
#include "ICFramework.h"

struct FVector2D
{
	float x;
	float y;
};

struct Bone_Data
{
	//uint16 BoneType;

	// float = 4byte
	float PosX;
	float PosY;
	float PosZ;
	float RotX;
	float RotY;
	float RotZ;
	float RotW;

};

struct ROKOKO_DATA
{
	// ROKOKO STUDIO 에서 값을 어떻게 넘겨주는 지 봐야함
	// PLUGIN 에서 BONE 이나 애니메이션 어떤 값을 보내주는가 

	Bone_Data datas[BONENUMBER];
};

