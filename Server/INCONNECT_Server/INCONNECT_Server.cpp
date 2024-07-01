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

// Send Message to All Clients
void SendMessageAll(char* pszMessage, int nSize)
{
    std::list<SOCKET>::iterator it;

    ::EnterCriticalSection(&g_cs);
    for (it = g_listClient.begin(); it != g_listClient.end(); ++it)
        ::send(*it, pszMessage, nSize, 0);
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

    // Close Listen Socket
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
    while (1)
    {
        bResult = ::GetQueuedCompletionStatus(
            g_hIocp,                // IOCP Handle for Dequeue
            &dwTransferredSize,     // Recieved Size of Data
            (PULONG_PTR)&pSession,  // Saved Memory that Recieved Data
            &pWol,                  // OVERLAPPED Struct 
            INFINITE);              // Waiting Event Infinitely

        if (bResult == TRUE)
        {
            // GOOD Case
             
            // 1. Client Close Socket and Disconnect Normally
            if (dwTransferredSize == 0)
            {
                CloseClient(pSession->hSocket);
                delete pWol;
                delete pSession;
                ErrorHandler("\tGQCS: Client Disconnect Normally");
            }

            // 2. Receive Data from Client
            else
            {
                SendMessageAll(pSession->buffer, dwTransferredSize);
                puts(pSession->buffer);
                memset(pSession->buffer, 0, sizeof(pSession->buffer));

                // Assign to IOCP again
                DWORD dwReceiveSize = 0;
                DWORD dwFlah        = 0;
                WSABUF wsaBuf       = { 0 };
                wsaBuf.buf = pSession->buffer;
                wsaBuf.len = sizeof(pSession->buffer);


                ::WSARecv(
                    pSession->hSocket,        // Client Socket Handle
                    &wsaBuf,                  // Address of WSABUF Struct Array
                    1,                        // Number of Array Element 
                    &dwReceiveSize,
                    &dwFlag,
                    pWol,
                    NULL);

            


                if (::WSAGetLastError() != WSA_IO_PENDING)
                    ErrorHandler("\tGQCS: ERROR WSARecv()");

            }
        }
        else
        {
            // Unusually 

            // 3. Returned Case, Can't Get Finish Packet from Finish queue 
            if (pWol == NULL)
            {
                // Close IOCP Handle Case (Involved Quit Server)
                ErrorHandler("\tGQCS: IOCP Handle Closed");
                break;
            }

            // 4. Client Unusual QUIT AND SERVER QUIT FIRST
            else
            {
                if (pSession != NULL)
                {
                    CloseClient(pSession->hSocket);
                    delete pWol;
                    delete pSession;
                }

                ErrorHandler("\tGQCS: Client Unusual QUIT AND SERVER QUIT FIRST");
            }

        }
        
    }

    puts("[IOCP Worker Thread Close]");
    return 0;
}

DWORD WINAPI ThreadAcceptLoop(LPVOID pParam)
{
    LPWSAOVERLAPPED	pWol = NULL;
    DWORD			dwReceiveSize, dwFlag;
    USERSESSION* pNewUser;
    int				nAddrSize = sizeof(SOCKADDR);
    WSABUF			wsaBuf;
    SOCKADDR		ClientAddr;
    SOCKET			hClient;
    int				nRecvResult = 0;

    while ((hClient = ::accept(g_hSocket,
        &ClientAddr, &nAddrSize)) != INVALID_SOCKET)
    {
        puts("New Client Connected.");
        ::EnterCriticalSection(&g_cs);
        g_listClient.push_back(hClient);
        ::LeaveCriticalSection(&g_cs);

        // Create Session Object about New Client
        pNewUser = new USERSESSION;
        ::ZeroMemory(pNewUser, sizeof(USERSESSION));
        pNewUser->hSocket = hClient;

        // Create OVERLAPPED Struct for Async Recv 
        pWol = new WSAOVERLAPPED;
        ::ZeroMemory(pWol, sizeof(WSAOVERLAPPED));

        // Connect (Connected)Client Socket Handle to ICOP
        ::CreateIoCompletionPort((HANDLE)hClient, g_hIocp,
            (ULONG_PTR)pNewUser,        //KEY
            0);

        dwReceiveSize = 0;
        dwFlag = 0;
        wsaBuf.buf = pNewUser->buffer;
        wsaBuf.len = sizeof(pNewUser->buffer);

        // Async Recv that Info from Client
        nRecvResult = ::WSARecv(hClient, &wsaBuf, 1, &dwReceiveSize,
            &dwFlag, pWol, NULL);
        if (::WSAGetLastError() != WSA_IO_PENDING)
        ErrorHandler("ERROR: WSARecv() != WSA_IO_PENDING");
    }
    return 0;
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
        ErrorHandler("ERROR: Unsigned Ctrl+C Event.");

    // Create IOCP
    g_hIocp = ::CreateIoCompletionPort(
        INVALID_HANDLE_VALUE,       // Nothing Connect File
        NULL,                         // None Basic Handle
        0,                            // Not Matched Key(identify)
        0);                           // OS judged Thread Number
    
    if (g_hIocp == NULL)
    {
        ErrorHandler("ERROR : Can't Create IOCP");
        return 0;
    }

    // Create IOCP Thread 
    HANDLE hThread;
    DWORD dwThreadID;
    for (int i = 0; i < MAX_THREAD_CNT; ++i)
    {
        dwThreadID = 0; 
        // Received String From Client
        hThread = ::CreateThread(NULL, // Guard Property 
            0,                            // Stack Memory = 1MB
            ThreadComplete,             // Want Processing Function Name as Thread
            (LPVOID)NULL,
            0,                            // Create Flag is Basic Value
            &dwThreadID);                // Saving Address of Created Thread ID
            
        ::CloseHandle(hThread);
    }

    // Create Server Listen Socket 
    g_hSocket = ::WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP,
                       NULL, 0, WSA_FLAG_OVERLAPPED);

    // Bind() / Listen () 
    SOCKADDR_IN addrsvr;
    addrsvr.sin_family = AF_INET;
    addrsvr.sin_addr.S_un.S_addr = ::htonl(INADDR_ANY);
    addrsvr.sin_port = ::htons(25000);

    if (::bind(g_hSocket,
        (SOCKADDR*)&addrsvr, sizeof(SOCKADDR_IN)) == SOCKET_ERROR)
    {
        ErrorHandler("ERROR: Port is aready run");
        ReleaseServer();
        return 0;
    }

    if (::listen(g_hSocket, SOMAXCONN) == SOCKET_ERROR)
    {
        ErrorHandler("ERROR: Can't Change Listen Mode");
        ReleaseServer();
        return 0;
    }

    // Accept Client Connect repeat
    hThread = ::CreateThread(NULL, 0, ThreadAcceptLoop,
        (LPVOID)NULL, 0, &dwThreadID);
    ::CloseHandle(hThread);

    // Waiting for _tmain() Can't return 
    puts("*** 채팅서버를 시작합니다! ***");
    while (1)
        getchar();

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
