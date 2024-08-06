#include "User.h"
#include "Core.h"
#include "ICGlobal.h"

#include "ICProtocol.h"
#include "ICPacket.h"

User::User(void)
{
	// Initialize var
	mhSocket = NULL;
	mRecvWrite = 0;

	mSendSize = 0;

	mUID = 0;
	mIndex = INVALID_VALUE;

	memset(mName, 0x00, sizeof(mName));
}

User::~User(void)
{
	Clear();
}

void User::Clear()
{
	if (mhSocket != NULL)
	{
		shutdown(mhSocket, SD_SEND);
		shutdown(mhSocket, SD_RECEIVE);
	}
	SAFE_CLOSESOCKET(mhSocket);

	mRecvWrite = 0;
	memset(mRecvBuffer, 0x00, sizeof(mRecvBuffer));

	mSendSize = 0;
	memset(mSendBuffer, 0x00, sizeof(mSendBuffer));

	memset(mName, 0x00, sizeof(mName));

	mUID = 0;
	mIndex = INVALID_VALUE;

	mThreadNum = INVALID_VALUE;
}

bool User::Init(int index, SOCKET sock, sockaddr_in ip)
{
	if (index < 0 || index >= MAX_USER)
		return false;

	Clear();

	mhSocket = sock;
	mIndex = index;

	mThreadNum = index / gUserper;

	//SendConnect();

	int t = mThreadNum;
	if (t < 0 || t > MAX_QUEUE)
		return false;

	return true;
}

void User::LogOut()
{
	if (mhSocket) {
		int t = mThreadNum;
		if (t < 0 || t > MAX_QUEUE)
			return;
	}
	Clear();
}

void User::EmptyRecvBuffer()
{
	memset(mRecvBuffer, 0x00, sizeof(mRecvBuffer));
	mRecvWrite = 0;
}

int User::FlushSendBuffer()
{
	int sendsize = 0;
	do
	{
		sendsize = send(mhSocket, mSendBuffer, mSendSize, 0);
		if (sendsize == SOCKET_ERROR)
		{
			uint32 error = WSAGetLastError();
			return error;
		}
		else
		{
			memmove(mSendBuffer, &mSendBuffer[sendsize], mSendSize - sendsize);
			mSendSize -= sendsize;
		}
	} while (mSendSize);

	return mSendSize;
}

bool User::AddSendBuffer(char* buff, int size)
{
	if( buff == NULL)
	   return false;
	if (mSendSize + size >= MAX_SEND)
	{
		Clear();
		return false;
	}

	memcpy(&mSendBuffer[mSendSize], buff, size);
	mSendSize += size;
	return true;
}

void User::Send(char* buff, int size)
{
	if (mhSocket == NULL) {
		return;
	}
	if (buff == NULL)
		return;

	int sendsize, error = 0;
	if (mSendSize <= 0) {// Only once Case : Queue Empty

		do {
			sendsize = send(mhSocket, buff, size, 0);

			if (sendsize < 0) {
				AddSendBuffer(buff, size);
				break;
			}
			else
			{
					buff = buff + sendsize; // 버퍼의 위치를 send 한 만큼 뒤로 밈
					size -= sendsize;       // 패킷 사이즈를 보낸만큼 빼준다.
			}
		} while (size);                 // size가 0이 될 때까지 보낸다.

	}
	else {// 큐가 비어있지 않다면 보낼 데이터를 큐에 쌓고, 
			// 버퍼를 초과하지 않았다면 FlushBuffer();를 호출해서 처리한다.
		if (AddSendBuffer(buff, size)) {
			FlushSendBuffer();
		}
		else {
			FlushSendBuffer();
		}
	}
}

void User::Recv()
{
	if (mhSocket == NULL)
		return;
	if (mIndex < 0)
		return;

	int size = 0;
	if (mRecvWrite < MAX_RECV)
		size = recv(mhSocket, &mRecvBuffer[mRecvWrite], MAX_RECV - mRecvWrite, 0);
	    //stTestPacket header;
	    //memcpy(&header, mRecvBuffer, sizeof(stTestPacket));

	if (size > 0) {
		// ADD at Current RecvBuffer's Length
		mRecvWrite += size;

		if (mRecvWrite >= MAX_RECV) {
			puts("User Buffer is Full");
		}

		while (mRecvWrite >= HEADSIZE) {
			stHeader header;
			memcpy(&header, mRecvBuffer, HEADSIZE);
			// Why didn't use ?
			//if (header.nID >= PROTOCOL_END || header.nID <= PROTOCOL_START) {
			//	Clear();
			//	EmptyRecvBuffer();
			//	return;
			//}


			if (header.nSize <= 0) {
				EmptyRecvBuffer();
				return;
			}
			int iCheckSum = header.nType + header.nSize + header.nID;
			if (header.nCheckSum != iCheckSum) {
				EmptyRecvBuffer();
				return;
			}

			if (mRecvWrite >= header.nSize) {
				Parse(header.nID, mRecvBuffer);
				memmove(mRecvBuffer, &mRecvBuffer[header.nSize], mRecvWrite);
				mRecvWrite -= header.nSize;
			}
			else {
				break;
			}
		}
	}
}

void User::Parse(int protocol, char* packet)
{
	switch (protocol)
	{
	case prLoginReq:	RecvLoginReq(packet);	break;
	case prBoneData:	RecvBoneData(packet);	break;
   

	//default:			SendDefault(packet);	break;
	}


}

void User::RecvLoginReq(char* packet)
{
	//req 처리는 나중에
	stLoginAck ack;

	char buffer[64];	
	memset(buffer, 0x00, sizeof(buffer));


	ack.UID = g_User.GetUserCount();
	ack.Result = 1; // SUCCESS
	memcpy( buffer, &ack, sizeof(stLoginAck));
	g_User.SendAll(buffer, sizeof(stLoginAck));
}

void User::RecvBoneData(char* packet)
{
	stBoneData req;
	memcpy(&req, packet, sizeof(stBoneData));

	// Test 끝나면 
	// SendOther()
	g_User.SendAll(packet, sizeof(stBoneData));
	puts("Recv And Send All Packet");
}
