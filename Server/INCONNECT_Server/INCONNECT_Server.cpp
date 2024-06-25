// INCONNECT_Server.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "stdafx.h"

#include <winsock2.h>
#pragma comment(lib, "ws2_32")
//::TranmitFile() 함수를 사용하기 위한 헤더와 라이브러리 설정
#include <Mswsock.h>
#include <windows.h>
#include <list>
#include <iterator>
#pragma comment(lib, "Mswsock")

typedef struct _USERSESSION
{
    SOCKET	hSocket;
    char	buffer[8192];	//8KB
} USERSESSION;


// 클라이언트 처리를 위한 작업자 스레드 개수.
#define MAX_THREAD_CNT		4

CRITICAL_SECTION	g_cs;			//스레드 동기화 객체.
SOCKET				g_hSocket;		//서버의 리슨 소켓.
std::list<SOCKET>	g_listClient;	//연결된 클라이언트 소켓 리스트.
HANDLE	g_hIocp;					//IOCP 핸들

// 전송할 모션 데이터 정보를 담기위한 구조체 
typedef struct ROKOKO_DATA
{
  // ROKOKO STUDIO 에서 값을 어떻게 넘겨주는 지 봐야함
  // PLUGIN 에서 BONE 이나 애니메이션 어떤 값을 보내주는가 

    char szName[_MAX_FNAME];
    DWORD dwSize;


}ROKOKO_DATA;


void SendChattingMessage(char* pszParam)
{
    int nLength = strlen(pszParam);
    std::list<SOCKET>::iterator it;

    ::EnterCriticalSection(&g_cs);
    for (it = g_listClient.begin(); it != g_listClient.end(); ++it)
        ::send(*it, pszParam, sizeof(char) * (nLength + 1), 0);
    ::LeaveCriticalSection(&g_cs);
}


void ErrorHandler(const char* pszMessage)
{
    printf("ERROR : %s\n", pszMessage);
    ::WSACleanup();
    exit(1);
}

// Close all socket and Listen server
void CloseAll()
{
    std::list<SOCKET>::iterator it;

    ::EnterCriticalSection(&g_cs);
    for (it = g_listClient.begin(); it != g_listClient.end(); ++it)
    {
        ::shutdown(*it, SD_BOTH);
        ::closesocket(*it);
    }
    ::LeaveCriticalSection(&g_cs);
}

/////////////////////////////////////////////////////////////////////////
void CloseClient(SOCKET hSock)
{
    ::shutdown(hSock, SD_BOTH);
    ::closesocket(hSock);

    ::EnterCriticalSection(&g_cs);
    g_listClient.remove(hSock);
    ::LeaveCriticalSection(&g_cs);
}

void ReleaseServer(void)
{
    // Close All Client Connect
    CloseAll();
    ::Sleep(500);

    //Listen 소켓을 닫는다.
    ::shutdown(g_hSocket, SD_BOTH);
    ::closesocket(g_hSocket);
    g_hSocket = NULL;

    //IOCP 핸들을 닫는다. 이렇게 하면 GQCS() 함수가 FALSE를 반환하며
    //:GetLastError() 함수가 ERROR_ABANDONED_WAIT_0을 반환한다.
    //IOCP 스레드들이 모두 종료된다.
    ::CloseHandle(g_hIocp);
    g_hIocp = NULL;

    //IOCP 스레드들이 종료되기를 일정시간 동안 기다린다.
    ::Sleep(500);
    ::DeleteCriticalSection(&g_cs);
}

// Quit server when press Ctrl + C
BOOL CtrlHandler(DWORD dwType)
{
    if (dwType == CTRL_C_EVENT)
    {
        ReleaseServer();

        puts("*** 채팅서버를 종료합니다! ***");
        ::WSACleanup();
        exit(0);
        return TRUE;
    }

    return FALSE;
}

DWORD WINAPI ThreadComplete(LPVOID pParam)
{
    DWORD             dwTransferredSize = 0;
    DWORD             dwFlag = 0;
    USERSESSION*     pSession = NULL;
    LPWSAOVERLAPPED  pWol = NULL;
    BOOL              bResult;


    puts("[IOCP Worker Thread Start]");


}




DWORD WINAPI ThreadFunction(LPVOID hClient)
{
    char szBuffer[128] = { 0 };
    int nReceive = 0;
   
    while ((nReceive = ::recv((SOCKET)hClient,
        szBuffer, sizeof(szBuffer), 0)) > 0)
    {
        puts(szBuffer);
        // Send to All Client that received char
        //SendChattingMessage(szBuffer);
        memset(szBuffer, 0, sizeof(szBuffer));
    }

    //g_listClient.remove((SOCKET)hClient);
    //::closesocket((SOCKET)hClient);
    return 0;
}



int main()
{
    // Init Winsock
    WSADATA wsa = { 0 };
    if (::WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
        ErrorHandler("Can't Initialize Winsock.");

    // Create CriticalSection
    ::InitializeCriticalSection(&g_cs);

    // Press Ctrl+C, Binding FUNCTION 
    if (::SetConsoleCtrlHandler(
        (PHANDLER_ROUTINE)CtrlHandler, TRUE) == FALSE)
        puts("ERROR: Unsigned Ctrl+C Event.");

    // Create Listen Socket
    SOCKET hSocket = ::socket(AF_INET, SOCK_STREAM, 0);
    if (hSocket == INVALID_SOCKET)
        ErrorHandler("Can't Create Listen Socket.");

    // Port Binding 
    SOCKADDR_IN svraddr = { 0 };
    svraddr.sin_family = AF_INET;
    svraddr.sin_port = htons(25000);
    svraddr.sin_addr.S_un.S_addr = htonl(INADDR_ANY);
    if (::bind(hSocket,
        (SOCKADDR*)&svraddr, sizeof(svraddr)) == SOCKET_ERROR)
        ErrorHandler("소켓에 IP주소와 포트를 바인드 할 수 없습니다.");
    
    // Change Listen Mode
    if (::listen(hSocket, SOMAXCONN) == SOCKET_ERROR)
        ErrorHandler("Can't Change Listen Mode");
    puts("Start INCONNECT Server.");

    // Accept Client and Create New Socket(Open)
    SOCKADDR_IN clientaddr = { 0 };
    int nAddrLen = sizeof(clientaddr);
    SOCKET hClient = ::accept(hSocket,
        (SOCKADDR*)&clientaddr, &nAddrLen);
    if (hClient == INVALID_SOCKET)
        ErrorHandler("Can't Create I/O Socket.");
    puts("Connect Client");

    DWORD dwThreadID = 0;
    HANDLE hThread;

    hThread = ::CreateThread(NULL,
        0,
        ThreadFunction,
        (LPVOID)hClient,
        0,
        &dwThreadID);
        
    //::CloseHandle(hThread);

    // Waiting for disconnecting Client.
    //::recv(hClient, NULL, 0, 0);
    //puts("Disconnect Client.");
    
    ::closesocket(hClient);
    ::closesocket(hSocket);
    ::WSACleanup();
    return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
