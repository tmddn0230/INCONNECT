#pragma once
#include "User.h"

class UserManager
{
public:

	User mUser[ MAX_USER ];
	int mPreIndex;
	int mChatIndex;

	bool mbStart;

public:
	UserManager(void);
	~UserManager(void);

	bool AddUser(SOCKET sock, sockaddr_in ip);
	void DelUser(int index);
	User* GetUser(int uid);

	void SendOther(int index, char* buff, int size);
	void SendAll(char* buff, int size);

	void SendMsg(char* buff, int size, int FID);

	int GetUserCount();
};

