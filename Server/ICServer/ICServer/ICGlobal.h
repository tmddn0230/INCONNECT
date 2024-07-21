#pragma once

#include "ICInclude.h"
#include "ICFramework.h"
#include "UserManager.h"

// Variable
extern HWND gMainWnd;
extern HWND gListboxWindow;
extern HFONT gArialFont;
extern int gUserper; // User per 1Thread, 0~50?
extern int MAX_QUEUE;
extern int MAX_THREAD;
extern int gbUseUDP;

// TCP
extern char gHostName[256];
extern char gIP[20];
extern int gServerPort;

extern SOCKET gServerSocket;
extern HANDLE gServerHandle;

extern int MAX_THREAD;
extern WSAEVENT gEvent[ MAX_USER ];
extern HANDLE gUserHandle[ MAX_USER ];
extern int gServerNum;

// UDP 


// User
extern UserManager g_User;

// Function
void Log(const char* format, ...);
void DebugLog(char* filename, const char* format, ...);

std::string AnsiToUtf8( std::string strAnsi );
std::string Utf8ToAnsi( std::string strUTF8 );
std::wstring Utf8ToWchar( std::string strAnsi );
std::string WcharToAnsi( std::wstring strWchar );
std::string WcharToUtf8( std::wstring strWchar );
