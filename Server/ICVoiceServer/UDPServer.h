#pragma once
#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>

#pragma comment(lib, "Ws2_32.lib")

#define DEFAULT_BUFLEN 512
#define DEFAULT_PORT "27015"

class UDPServer
{
public:
    UDPServer();
    ~UDPServer();
    bool Initialize();
    void StartListening();
    void Cleanup();

private:
    SOCKET ListenSocket;
    struct addrinfo* result;
    struct addrinfo hints;
};
