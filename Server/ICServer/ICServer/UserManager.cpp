#include "ICInclude.h"
#include "ICGlobal.h"
#include "ICDefine.h"
#include "UserManager.h"


UserManager::UserManager(void)
{
	mPreIndex = -1;
	mChatIndex = 0;
	mbStart = false;
}

UserManager::~UserManager(void)
{
}

bool UserManager::AddUser(SOCKET sock, sockaddr_in ip)
{
	int i;
	for (i = 0; i < MAX_USER; ++i)
	{
		if (mUser[i].mhSocket != NULL)
			continue;

		WSAResetEvent(gEvent[i]);
		WSAEventSelect(sock, gEvent[i], FD_READ | FD_WRITE | FD_CLOSE);

		BOOL opt_val = TRUE;
		setsockopt(sock, IPPROTO_TCP, TCP_NODELAY, (char*)&opt_val, sizeof(opt_val));

		DWORD recvsize = MAX_RECV;
		DWORD sendsize = MAX_SEND;
		setsockopt(sock, SOL_SOCKET, SO_SNDBUF, (const char*)&recvsize, sizeof(recvsize));
		setsockopt(sock, SOL_SOCKET, SO_RCVBUF, (const char*)&sendsize, sizeof(sendsize));

		struct linger Linger;
		Linger.l_onoff = 1; //¸µ°Å ²ô±â, Time Wait
		Linger.l_linger = 0;
		setsockopt(sock, SOL_SOCKET, SO_LINGER, (const char*)&Linger, sizeof(Linger));


		mUser[i].Init(i, sock, ip);

		printf("AddUser: %d %d.%d.%d.%d", i,
			ip.sin_addr.S_un.S_un_b.s_b1,
			ip.sin_addr.S_un.S_un_b.s_b2,
			ip.sin_addr.S_un.S_un_b.s_b3,
			ip.sin_addr.S_un.S_un_b.s_b4);

		return true;

	}
	return false;
}

void UserManager::DelUser(int index)
{
	if (index < 0 || index >= MAX_USER)
		return;

	mUser[index].LogOut();
}

User* UserManager::GetUser(int uid)
{
	if (uid <= 0)
		return NULL;

	int i;
	for (i = 0; i < MAX_USER; ++i)
	{
		if (mUser[i].mUID == uid)
			return &mUser[i];
	}
	return NULL;
}

void UserManager::SendOther(int index, char* buff, int size)
{
	int i;
	for (i = 0; i < MAX_USER; ++i)
	{
		if (i == index)
			continue;

		mUser[i].Send(buff, size);
	}
}

void UserManager::SendAll(char* buff, int size)
{
	int i;
	for (i = 0; i < MAX_USER; ++i)
	{
		mUser[i].Send(buff, size);
	}
}

void UserManager::SendMsg(char* buff, int size, int FID)
{
	int i;
	for (i = 0; i < MAX_USER; ++i)
	{
		mUser[i].Send(buff, size);
	}
}

int UserManager::GetUserCount()
{
	int i;
	int nCount = 0;
	for (i = 0; i < MAX_USER; ++i)
	{
		if (mUser[i].mhSocket == NULL)
			continue;
		nCount++;
	}
	return nCount;
}
