#include "UDPServer.h"

UDPServer::UDPServer() : ListenSocket(INVALID_SOCKET), result(NULL)
{
    ZeroMemory(&hints, sizeof(hints));
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_DGRAM;
    hints.ai_protocol = IPPROTO_UDP;
    hints.ai_flags = AI_PASSIVE;
}

UDPServer::~UDPServer()
{
    Cleanup();
}

bool UDPServer::Initialize()
{
    WSADATA wsaData;
    int iResult;

    // Initialize Winsock
    iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0)
    {
        std::cerr << "WSAStartup failed with error: " << iResult << std::endl;
        return false;
    }

    // Resolve the local address and port to be used by the server
    iResult = getaddrinfo(NULL, DEFAULT_PORT, &hints, &result);
    if (iResult != 0)
    {
        std::cerr << "getaddrinfo failed with error: " << iResult << std::endl;
        WSACleanup();
        return false;
    }

    // Create a SOCKET for the server to listen for client connections
    ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
    if (ListenSocket == INVALID_SOCKET)
    {
        std::cerr << "Error at socket(): " << WSAGetLastError() << std::endl;
        freeaddrinfo(result);
        WSACleanup();
        return false;
    }

    // Setup the UDP listening socket
    iResult = bind(ListenSocket, result->ai_addr, (int)result->ai_addrlen);
    if (iResult == SOCKET_ERROR)
    {
        std::cerr << "bind failed with error: " << WSAGetLastError() << std::endl;
        freeaddrinfo(result);
        closesocket(ListenSocket);
        WSACleanup();
        return false;
    }

    freeaddrinfo(result);
    return true;
}

void UDPServer::StartListening()
{
    char recvbuf[DEFAULT_BUFLEN];
    int iResult;
    struct sockaddr_in clientAddr;
    int clientAddrLen = sizeof(clientAddr);

    std::cout << "Listening for incoming connections..." << std::endl;

    while (true)
    {
        iResult = recvfrom(ListenSocket, recvbuf, DEFAULT_BUFLEN, 0, (sockaddr*)&clientAddr, &clientAddrLen);
        if (iResult == SOCKET_ERROR)
        {
            std::cerr << "recvfrom failed with error: " << WSAGetLastError() << std::endl;
            break;
        }

        recvbuf[iResult] = '\0'; // Null-terminate the received data
        std::cout << "Received message: " << recvbuf << std::endl;
    }
}

void UDPServer::Cleanup()
{
    if (ListenSocket != INVALID_SOCKET)
    {
        closesocket(ListenSocket);
        ListenSocket = INVALID_SOCKET;
    }
    WSACleanup();
}
