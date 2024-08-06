#pragma once
#include "ICInclude.h"
#include "ICDefine.h"

class User
{
public:

	SOCKET mhSocket;

	int mThreadNum;
	int mIndex;
	int mUID;

	int mRecvWrite;
	char mRecvBuffer[ MAX_RECV ];

	int mSendSize;
	char mSendBuffer[ MAX_SEND ];

	char mName[32];


public:
	User( void );
	~User( void );
	void Clear();

	bool Init(int index, SOCKET sock, sockaddr_in ip);

	void LogOut();

	void EmptyRecvBuffer();

	int FlushSendBuffer();
	bool AddSendBuffer(char* buff, int size);

	void Send(char* buff, int size);
	void Recv();
	void Parse(int protocol, char* packet);

public: //RECV
	void RecvLoginReq(char* packet);
	void RecvBoneData(char* packet);
	void RecvTransPos(char* packet);
	void RecvMatchReq(char* packet);
	void RecvFirstAtr(char* packet);
	void RecvMBTI    (char* packet);
	void RecvEmoticon(char* packet);
};

