// ICServer.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "ICDefine.h"
#include "ICGlobal.h"
#include "ICServer.h"

// Quit server when press Ctrl + C
BOOL CtrlHandler(DWORD dwType)
{
    if (dwType == CTRL_C_EVENT)
    {
        gServerHandle = NULL;
        // 연결된 모든 클라이언트 및 리슨 소켓을 닫고 프로그램을 종료한다.
        ::shutdown(gServerSocket, SD_BOTH);
        // 연결 리스트에 등록된 모든 정보를 삭제한다.

        for (int i = 0; i < g_User.GetUserCount(); i++) {
            g_User.mUser[i].LogOut();
            puts("모든 클라이언트 연결을 종료했습니다.");
        }
       
        //클라이언트와 통신하는 스레드들이 종료되기를 기다린다.
        ::Sleep(100);
        ::closesocket(gServerSocket);

        //윈속 해제
        ::WSACleanup();
        exit(0);
        return TRUE;
    }

    return FALSE;
}

int main() {
    
    Init();

    //----------------------------------------------------------------------------------------------------------
    // Circulation Thread number
    //----------------------------------------------------------------------------------------------------------

    MAX_THREAD = MAX_USER / gUserper; // Userper is 20
    MAX_QUEUE = MAX_THREAD;

    // Start WinSock
    WinSockStart();

    // Create Event
    int i;
    for (i = 0; i < MAX_USER; ++i)
    {
        gEvent[i] = WSACreateEvent();
    }

    // Accept Thread
    gServerHandle = (HANDLE)_beginthreadex(NULL, 0, ServerThread, NULL, 0, NULL);

    // User Thread
    for (i = 0; i < MAX_THREAD; ++i)
    {
        gUserHandle[i] = NULL;
        gUserHandle[i] = (HANDLE)_beginthreadex(NULL, 0, UserThread, (void*)i, 0, NULL);
    }


    // 이부분을 버튼 누르면 종료되는 이벤트를 넣어서 대체해야대
   //MSG msg;
   //
   //while (GetMessage(&msg, nullptr, 0, 0))
   //{
   //    if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
   //    {
   //        TranslateMessage(&msg);
   //        DispatchMessage(&msg);
   //    }
   //}
   //
   //return (int)msg.wParam;
       // Press Ctrl+C, Binding FUNCTION 
    if (::SetConsoleCtrlHandler(
        (PHANDLER_ROUTINE)CtrlHandler, TRUE) == FALSE)
        puts("ERROR: Unsigned Ctrl+C Event.");
    
    while (CtrlHandler)
    {

    }

    return 0;
}


void Init()
{
	gServerPort = 25000;
	gServerNum = 0;
	gUserper = 20;
	//gbUseUDP = 0;
}

BOOL WinSockStart()
{
	WSAData wsaData;
	int     error;
    
	error = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (error == SOCKET_ERROR)
	{
		WSACleanup();
		return FALSE;
	}
    
    
    char str[256]; memset(str, 0x00, sizeof(str));
    
    PHOSTENT phostent;
    IN_ADDR in;
    if ((phostent = gethostbyname(gHostName)) == NULL)
    {
        sprintf(str, "gethostbyname() generated error %d", WSAGetLastError());
        puts(str);
    }
    else
    {
        memcpy(&in, phostent->h_addr, 4);
        memset(gIP, 0x00, sizeof(gIP));
        sprintf(gIP, "%s", inet_ntoa(in));
        sprintf(str, "Host Name Is [ %s ] IP : [%s]", phostent->h_name, gIP);
        puts(str);
    }
    return TRUE;
}

unsigned __stdcall ServerThread(void* pArg)
{
    char szBuffer[1024];	memset(szBuffer, 0x00, sizeof(szBuffer));

    gServerSocket = socket(AF_INET, SOCK_STREAM, 0);

    if (gServerSocket == INVALID_SOCKET)
    {
        sprintf(szBuffer, "Socket error code=%x", WSAGetLastError());
        puts(szBuffer);
        return 0;
    }

    //나겔알고리즘 비적용
    BOOL opt_val = TRUE;
    setsockopt(gServerSocket, IPPROTO_TCP, TCP_NODELAY, (char*)&opt_val, sizeof(opt_val));
    DWORD size = 0x8000;
    setsockopt(gServerSocket, SOL_SOCKET, SO_SNDBUF, (const char*)&size, sizeof(size));
    setsockopt(gServerSocket, SOL_SOCKET, SO_RCVBUF, (const char*)&size, sizeof(size));

    struct sockaddr_in sa;
    memset(&sa, 0, sizeof(sa));
    sa.sin_family = AF_INET;
    sa.sin_addr.s_addr = htonl(INADDR_ANY);
    sa.sin_port = htons(gServerPort);

    //Bind
    if (::bind(gServerSocket, (struct sockaddr*)&sa, sizeof(sa)) != 0)
    {
        sprintf(szBuffer, "bind error code=%d", WSAGetLastError());
        puts(szBuffer);
        return 0;
    }

    //Listen
    if (listen(gServerSocket, 500) != 0)
    {
        sprintf(szBuffer, "listen error code=%d", WSAGetLastError());
        puts(szBuffer);
        return 0;
    }

    while (gServerHandle)
    {
        struct sockaddr_in ca;
        int clientAddressLength = sizeof(ca);
        int nLength = sizeof(ca);

        SOCKET socket = accept(gServerSocket, (struct sockaddr*)&ca, &nLength);

        if (socket == INVALID_SOCKET)
        {
            closesocket(socket);

            puts("Failed Socket Create");
            continue;
        }
        //유저등록에 실패하면 소켓을 닫아버린다..

       if (g_User.AddUser(socket, ca) == false)
       {
           closesocket(socket);
       
           printf("ADDUser Fail %d.%d.%d.%d",
               ca.sin_addr.S_un.S_un_b.s_b1,
               ca.sin_addr.S_un.S_un_b.s_b2,
               ca.sin_addr.S_un.S_un_b.s_b3,
               ca.sin_addr.S_un.S_un_b.s_b4);
       }

        Sleep(1);
    }
    return 0;

}

unsigned __stdcall UserThread(void* pArg)
{
    WSANETWORKEVENTS events;

    DWORD dwReturn = 0, dwRet = 0;

    //int nread;
    int ThreadArray = (int)pArg;
    int UserArray = (ThreadArray * gUserper);
    int i = 0;
    int inum = 0;
    //Log("UserThread %d", ThreadArray );
    while (gUserHandle[ThreadArray])
    {
        //64개..
        dwReturn = WSAWaitForMultipleEvents(gUserper, &gEvent[UserArray], FALSE, WSA_INFINITE, FALSE);

        if (dwReturn != WSA_WAIT_FAILED)
        {
            for (i = 0; i < gUserper; ++i)
            {
                inum = UserArray + i;

                // UserManager
                if (g_User.mUser[inum].mhSocket)
                {
                
                    dwRet = WSAEnumNetworkEvents(g_User.mUser[inum].mhSocket, gEvent[inum], &events);
                
                    if (dwRet == 0)
                    {
                        //FD_READ EVENT 면.
                        if ((events.lNetworkEvents & FD_READ) == FD_READ)
                        {
                            g_User.mUser[inum].Recv();
                        }
                        if ((events.lNetworkEvents & FD_WRITE) == FD_WRITE)
                        {
                            g_User.mUser[inum].FlushSendBuffer();
                        }
                        if ((events.lNetworkEvents & FD_CLOSE) == FD_CLOSE)
                        {
                            //접속 종료 처리
                            //Log("g_User.DelUser( inum %d );", inum );
                            g_User.DelUser(inum);
                        }
                    }
                }
            }
        }
        Sleep(1);
    }
    return 0;
}
