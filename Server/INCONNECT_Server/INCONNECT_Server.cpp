// INCONNECT_Server.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "stdafx.h"

#include <winsock2.h>
#pragma comment(lib, "ws2_32")
//::TranmitFile() 함수를 사용하기 위한 헤더와 라이브러리 설정
#include <Mswsock.h>
#pragma comment(lib, "Mswsock")

// 전송할 모션 데이터 정보를 담기위한 구조체 
typedef struct ROKOKO_DATA
{
  // ROKOKO STUDIO 에서 값을 어떻게 넘겨주는 지 봐야함
  // PLUGIN 에서 BONE 이나 애니메이션 어떤 값을 보내주는가 

    char szName[_MAX_FNAME];
    DWORD dwSize;


}ROKOKO_DATA;

void ErrorHandler(const char* pszMessage)
{
    printf("ERROR : %s\n", pszMessage);
    ::WSACleanup();
    exit(1);
}

int main()
{
    // Init Winsock
    WSADATA wsa = { 0 };
    if (::WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
        ErrorHandler("Can't Initialize Winsock.");

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


    // Waiting for disconnecting Client.
    ::recv(hClient, NULL, 0, 0);
    puts("Disconnect Client.");

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
