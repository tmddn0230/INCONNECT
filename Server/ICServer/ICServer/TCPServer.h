#pragma once

#include "ICInclude.h"
#include <WinSock2.h>

class TCPServer
{
public:
	SOCKET mhSocket;
 
	int mThreadNumber;

	int mIndex;
	int mUID;

	int mRecvWrite;
	char mRecvBuffer[MAX_RECV];

	int mSendSize;
	char mSendBuffer[MAX_SEND];

	int mGrade;
	int mName[32];

	
};

