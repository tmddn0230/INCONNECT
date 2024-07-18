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
	// ROKOKO STUDIO ���� ���� ��� �Ѱ��ִ� �� ������
	// PLUGIN ���� BONE �̳� �ִϸ��̼� � ���� �����ִ°� 

	Bone_Data datas[BONENUMBER];
};

