메타버스 경연대회를 위해 ‘VR 가상 데이팅 컨텐츠’를 제출하기 위한 사이드 프로젝트입니다.

👨‍🦱 참여인원 및 역할
- 김율호 : 기획
- 유승우 : TCP 서버 및 네트워크 클라이언트
- 정경언 : 컨텐츠
- 윤찬영 : 웹 서버 및 음성 채팅
- 김준혁 : UI
&nbsp;

&nbsp;
# <u>📰 기획 <u>
<details>
<summary> 💞두근두근 버츄얼 랜덤 매칭 데이팅💞 - 김율호 </summary>

[구현 목록] 
1. 아바타(유니티 에셋 스토어에 있는 무료 캐릭터 1종)
2. 광장(유니티 에셋 스토어에 있는 무료 맵 1종 = 캐릭터가 최초 스폰 되고, 이동하면서 음성대화 할 수 있는 정도의 공간)
3. UI(랜덤 매칭 및 이모티콘 등등 2D UI)
4. 랜덤 데이트 장소 : 카페(의자2개와 탁자1개로 이루어진 공간) - 프로토 타입 단계에서는 카페보다는 1대1 소통할 수 있는 장소 
- 카페에서는 최소한의 정보와 행동 할 수 있는 권한 제공(추후 개발 예정)
- 정해진 임무에 따라 사용자의 정보와 행동의 범위가 해금되어 더욱 자유롭게 상대를 알아 갈 수 있는 효과를 얻을 수 있다.(추후 개발 예정)
⇒ 정해진 행동 : 상대방의 요구사항,춤 or 노래
⇒ 해금 : 보이스 채팅, MBTI, 사용자 이름, 인스타 아이디 등등

[고려 해야 할 사항]

1. 캐릭터상 성별
2. 모션캡쳐를 이용한 이모티콘 

 3.  모션캡쳐는 손목까지 사용 , 컨트롤러를 잡고 진행하며 컨트롤러 대신 손모양을 랜더링

 4. 배그 감정표현 처럼 RADIAL UI 에서 선택하면 해당 제스쳐를 취하거나

1. 그 반대로 제스쳐를 취하면 이모티콘을 위에 출력
2. 광장 멀티 (20인 급)

⇒  광장에서는 1, 3인칭 : 모션 x , 아바타 컨트롤 + 채팅 

데이팅 시작할 때만 모션 + 1인칭

업적 캐릭터 머리위에 표기 (ex: 카페데이트 50명 하고온사람, 오락실10위안에들어온사람)

가까이 가서 컨트롤러 UI 상호작용?? ⇒ 데이트 신청, 인사하기 (이모티콘)

[Date_In_Persona_프로토타입_기획서_V.1.0.pptx](https://github.com/user-attachments/files/17892260/Date_In_Persona_._._V.1.0.pptx)<br>
[Date In Persona_카페시스템_기획서_V.1.0.pptx](https://github.com/user-attachments/files/17892262/Date.In.Persona_._._V.1.0.pptx)<br>
[Date In Persona_UI컨셉기획서_V.1.0.pdf](https://github.com/user-attachments/files/17892263/Date.In.Persona_UI._V.1.0.pdf)<br>
[Dating_시스템기획서_v01.pptx](https://github.com/user-attachments/files/17892264/Dating_._v01.pptx)<br>
</details>


&nbsp;

&nbsp;
# <u>💻 주요 기능 및 코드<u>

[Server]  : C++
<details>
 <summary> ICPacket : Used Packets - 유승우</summary>

## &nbsp;&nbsp;&nbsp;&nbsp;사용 패킷과 패킷관리 큐
    
```cpp

namespace Packet
{
    public enum enProtocol
    {
        PROTOCOL_START = 0,
        prConnectAck,
        prLoginReq, prLoginAck,
        prBoneData,
        prTransform,
        prMatchingReq, prMatchingAck,
        prFirstAttract,
        prMBTI,
        prSendEmo,
        prAfter,

        PROTOCOL_END
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]   // Pack 은 멤버간 정렬간격이 1바이트  멤버가 각각 1 바이트, 4바이트 라면 구조체의 크기는 5바이트란 얘기
                                                      // 설정하지 않으면 8바이트
    [System.Serializable]
    public struct StHeader
    {
        public ushort nID;
        public ushort nSize;
        public ushort nType;
        public ushort nCheckSum;

        public void SetHeader(int id, int len)
        {
            nType = 2222;
            nID = (ushort)id;
            nSize = (ushort)len;
            nCheckSum = (ushort)(nType + nID + nSize);
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public struct ICPacket_Bone 
    {

        public StHeader packetHeader;
        public int UID;
        public CoreBoneData bonedata;

        public void SetMotionProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + Marshal.SizeOf(bonedata) + sizeof(int);
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prBoneData, size);
        }     
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public struct ICPacket_Transform
    {

        public StHeader packetHeader;
        public int UID;
        float[] pos;
        float[] rot;

        public void Init()
        {
            pos = new float[3];
            rot = new float[4];
        }

        public void SetMotionProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + (sizeof(float) * pos.Length)  + (sizeof(float) * rot.Length);
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prTransform, size);
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class ICPacket_Login
    {
        public StHeader packetHeader;

        public int UID;
        public int Result;

        public void SetLoginProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int);
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prLoginReq, size);
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class ICPacket_Match
    {
        public StHeader packetHeader;
        public void SetMatchProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // 멤버에 따라 수정
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prMatchingReq, size);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class ICPacket_First
    {
        public StHeader packetHeader;
        public int UID;
        public int Score;
        
        public void SetAttractProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // 멤버에 따라 수정
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prFirstAttract, size);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class ICPacket_MBTI
    {
        public StHeader packetHeader;
        public int UID;
        public int MBTI; // mbti 1 ~ 12
        public void SetMBTIProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // 멤버에 따라 수정
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prMBTI, size);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class ICPacket_EMO
    {
        public StHeader packetHeader;
        public int UID;
        public int EMO;
        public void SetEmoProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // 멤버에 따라 수정
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prSendEmo, size);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class ICPacket_After
    {
        public StHeader packetHeader;
        public int UID;
        public int Result;
        public void SetAfterProtocol()
        {
            int size;
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // 멤버에 따라 수정
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prAfter, size);
        }
    }

    public class ICPacketQueue
    {
        private Queue<byte[]> queue;
        
        public ICPacketQueue()
        {
            queue = new Queue<byte[]>();
        }

        public void Enqueue(byte[] packet)
        {
            queue.Enqueue (packet);
            Debug.Log("Enqueue !");
        }

        public byte[] Dequeue()
        {
            if (queue.Count > 0)
            {
                byte[] packet = queue.Dequeue();
                Debug.Log("Dequeue !");
                return packet;
            }
            else
            {
                Debug.Log("Queue is Empty");
                return null;
            }
        }

    }
}

```


</details>

<details>
<summary> ICServer : Main Socket Server - 유승우 </summary>

 서버 특징<br>
  1. TCP Socket<br>
  2. Event Select<br>
## &nbsp;&nbsp;&nbsp;&nbsp;소켓 초기화 및 서버 실행
    
```cpp
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
```
## &nbsp;&nbsp;&nbsp;&nbsp;서버 쓰레드
    
```cpp
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
```

## &nbsp;&nbsp;&nbsp;&nbsp;유저 쓰레드
    
```cpp
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

```

## &nbsp;&nbsp;&nbsp;&nbsp;Send & Recv
    
```cpp
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
```
## &nbsp;&nbsp;&nbsp;&nbsp;유저 관리
    
```cpp
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
		Linger.l_onoff = 1; //링거 끄기, Time Wait
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

```
</details>

<details>
<summary> ICVoiceServer : Voice Server </summary>

   1. UDP Socket
</details>

<details>
 <summary> INCONNECT_Server : ICOP 서버 연습 - 유승우</summary>

## &nbsp;&nbsp;&nbsp;&nbsp;IOCP
    
```cpp
// IC
#include "ICProtocol.h";
#include "ICPacket.h";

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
        stHeader header;
        memcpy(&header, szBuffer, HEADSIZE);


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

```

</details>


&nbsp;
&nbsp;

[Client] : C#
<details>
 <summary> VR 조작 기능 구현 - 정경언</summary>

## &nbsp;&nbsp;&nbsp;&nbsp;UI 및 변수 선언

- VR 환경에서 UI 및 캐릭터 동작 처리를 위한 변수 선언.

- 캐릭터 이동, 회전 속도 설정 및 마이크 음소거 상태 관리.

```csharp

// UI Function
public GameObject mUIPanel;
public GameObject mEmotionPanel;
public Image MicImage;
private Sprite mMicOff;
private Sprite mMicOn;

// Character Function
public float mMoveSpeed = 3.0f;
public float mRotationSpeed = 100.0f;

private CharacterController mCharacterController;
private bool bIsMuted = false;

// Set Instance
private static ICInputManager instance;

public GameObject mOVRCam;
public bool Ismine = false;
public bool UseEmo = false;
public ICEmoticon ICEmoticon;
public ICSlider Slider;
public ICMBTI MBTI;
public ICResult Result;

```

## &nbsp;&nbsp;&nbsp;&nbsp;초기화

- CharacterController 컴포넌트를 초기화하고 마이크 상태 아이콘 로드.
    
```csharp

void Start()
{
    mCharacterController = GetComponent<CharacterController>();
    mMicOff = Resources.Load<Sprite>("Micoff");
    mMicOn = Resources.Load<Sprite>("MicOn");
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;VR 컨트롤러 입력 처리

- VR 컨트롤러의 입력을 처리하여 감정 패널, UI 패널 활성화 및 마이크 상태를 제어.
    
```csharp

void Update()
{
    if (!Ismine)
        return;
    MetaInputController();
}

private void MetaInputController()
{
    HandleLeftJoystickMovement();
    HandleRightJoystickRotation();

    // Right Controller
    if (OVRInput.GetActiveController() == OVRInput.Controller.RTouch)
    {
        // A Button (Open Emotion Panel)
        if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            if (UseEmo == false) return;
            mEmotionPanel.SetActive(!mEmotionPanel.activeSelf);
        }

        // B Button (Open 2D UI)
        if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            mUIPanel.SetActive(!mUIPanel.activeSelf);
        }
    }

    // Left Controller
    if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
    {
        // X Button - Mic Control
        if (OVRInput.GetUp(OVRInput.Button.Three, OVRInput.Controller.LTouch))
        {
            bIsMuted = !bIsMuted;
            MicImage.sprite = bIsMuted ? mMicOff : mMicOn;
        }
    }
}

```


</details>

<details>
 <summary> 서버를 통한 기능 구현 - 정경언</summary>

## &nbsp;&nbsp;&nbsp;&nbsp;캐릭터 스폰

- 서버에서 전달받은 UID를 기반으로 캐릭터를 생성하고 활성화.

```csharp

public void Actor_Spawn(int uid, int Result)
{
    if (Result != 1 || uid < 1 || uid > mActors.Length) return;
    if (m_UID == 0) m_UID = uid;
    if (spawnedActors == null) spawnedActors = new GameObject[mActors.Length];

    for (int i = 0; i < uid; i++)
    {
        if (spawnedActors[i] == null)
        {
            spawnedActors[i] = Instantiate(mActors[i], Vector3.zero, Quaternion.identity);
        }
    }

    foreach (var actor in spawnedActors)
    {
        var inputManager = actor?.GetComponent<ICInputManager>();
        if (inputManager != null) inputManager.Ismine = (actor == spawnedActors[m_UID - 1]);
    }
}


```

## &nbsp;&nbsp;&nbsp;&nbsp;감정표현 송/수신 

- 서버를 통한 감정표현 송/수신

```csharp

public void SendEmoticon(int i)
{
    ICNetworkManager.Instance.SendPacket_EMO(i);
}
public void ReceiveEmotion(int i)
{
    DefalutEmoAnim(emoticons[i]);
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;감정표현 애니메이션

- 감정 표현 애니메이션을 실행하여 화면에 표시

```csharp

IEnumerator MoveForward(Sprite emotion)
{
    transform.position = new Vector3(0f, 1.9f, 0f);
    Texture2D newAlbedoTexture = emotion.texture;
    mat.SetTexture("_MainTex", newAlbedoTexture);

    float startTime = Time.time;

    while (Time.time < startTime + 1f)
    {
        transform.Translate(Vector3.forward * Movespeed * Time.deltaTime);
        yield return null;
    }
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;상대방 점수 산정

- 스크롤 바를 이용해 서버를 통해 상대방의 점수를 산정.

```csharp

 public void SetScore()
 {
     text.text = "점수" + (silder.value * 100).ToString("F0")+ "점";
     string a = (silder.value * 100).ToString("F0");
     score = int.Parse(a);
 }

 public void SendScore()
 {
     ICNetworkManager.Instance.SendPacket_Attract(score);
 }

 public void Receive_Score(int Score)
 {
     if(Score < 70)
         Fail.SetActive(true);
        
     else
         Success.SetActive(true);
 }


```

## &nbsp;&nbsp;&nbsp;&nbsp;상대방과의 만남 선택

- 서버를 통해 결과값 주고 받기 구현.

```csharp

  public void Send_Result(int i) //0 성공 1 실패
  {
      ICNetworkManager.Instance.SendPacket_After(i);
  }

  public void Receive_Result(int i)
  {
      if(i == 0)
      {
          Continue.SetActive(true);
      }
      else
      {
          No_Continue.SetActive(true);
      }
  }

```

</details>

<details>
 <summary> 모션 데이터 동기화 - 정경언</summary>

## &nbsp;&nbsp;&nbsp;&nbsp;모션 데이터 구조체 정의

- 모션 데이터 동기화를 위한 구조체 정의

```csharp

public struct CoreBoneData
{
    // Body
    public float[] headPosition;
    public float[] headRotation;
    public float[] neckPosition;
    public float[] neckRotation;
    public float[] chestPosition;
    public float[] chestRotation;
    public float[] spinePosition;
    public float[] spineRotation;
    public float[] hipPosition;
    public float[] hipRotation;

    // Hands
    public float[] leftUpperArmPosition;
    public float[] leftUpperArmRotation;
    public float[] leftLowerArmPosition;
    public float[] leftLowerArmRotation;
    public float[] leftHandPosition;
    public float[] leftHandRotation;
    public float[] rightUpperArmPosition;
    public float[] rightUpperArmRotation;
    public float[] rightLowerArmPosition;
    public float[] rightLowerArmRotation;
    public float[] rightHandPosition;
    public float[] rightHandRotation;

    // Legs
    public float[] leftFootPosition;
    public float[] leftFootRotation;
    public float[] rightFootPosition;
    public float[] rightFootRotation;

    public void Init()
    {
        // Initialize all arrays
        headPosition = new float[3];
        headRotation = new float[4];
        neckPosition = new float[3];
        neckRotation = new float[4];
        ...
    }
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;본 데이터 수집

- HumanBodyBones를 기반으로 Unity의 Animator에서 본 위치와 회전을 가져옴

```csharp

private float[] GetPositionArray(HumanBodyBones bone)
{
    Transform boneTransform = m_actor.GetBone(bone);
    if (boneTransform == null) return new float[3];

    Vector3 position = boneTransform.position;
    return new float[] { position.x, position.y, position.z };
}

private float[] GetRotationArray(HumanBodyBones bone)
{
    Transform boneTransform = m_actor.GetBone(bone);
    if (boneTransform == null) return new float[4];

    Quaternion rotation = boneTransform.rotation;
    return new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
}


```

## &nbsp;&nbsp;&nbsp;&nbsp;데이터 업데이트

- 본 데이터를 Update에서 실시간으로 갱신

```csharp

void Update()
{
    if (m_actor == null) return;

    // Body
    m_coreBoneData.hipPosition = GetPositionArray(HumanBodyBones.Hips);
    m_coreBoneData.hipRotation = GetRotationArray(HumanBodyBones.Hips);
    m_coreBoneData.spinePosition = GetPositionArray(HumanBodyBones.Spine);
    m_coreBoneData.spineRotation = GetRotationArray(HumanBodyBones.Spine);
    m_coreBoneData.chestPosition = GetPositionArray(HumanBodyBones.Chest);
    m_coreBoneData.chestRotation = GetRotationArray(HumanBodyBones.Chest);
    m_coreBoneData.neckPosition = GetPositionArray(HumanBodyBones.Neck);
    m_coreBoneData.neckRotation = GetRotationArray(HumanBodyBones.Neck);
    m_coreBoneData.headPosition = GetPositionArray(HumanBodyBones.Head);
    m_coreBoneData.headRotation = GetRotationArray(HumanBodyBones.Head);

    // Hands
    m_coreBoneData.leftUpperArmPosition = GetPositionArray(HumanBodyBones.LeftUpperArm);
    m_coreBoneData.leftUpperArmRotation = GetRotationArray(HumanBodyBones.LeftUpperArm);
    m_coreBoneData.leftLowerArmPosition = GetPositionArray(HumanBodyBones.LeftLowerArm);
    ...
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;데이터 전송

- ICPacket_Bone 패킷 구조체에 본 데이터를 설정하고 네트워크 매니저를 통해 서버로 전송.

```csharp

void Datasend()
{
    if (mNetworkManager != null)
    {
        ICPacket_Bone bonepacket = new ICPacket_Bone();
        bonepacket.SetMotionProtocol();
        bonepacket.UID = 0; // UID 설정
        bonepacket.bonedata = m_coreBoneData; // 본 데이터 설정

        mNetworkManager.SendPacket_Bone(bonepacket);
    }
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;데이터 수신 및 업데이트

- 수신 데이터를 CoreBoneData에 반영 및 캐릭터의 본 데이터 갱신

```csharp

void testupdate()
{
    if (mNetworkManager != null)
    {
        mNetworkManager.GetReciever().GetDictionValue(1, m_coreBoneData);
        // UpdateCharacter 함수로 본 데이터 업데이트
        CharacterAnim.Instance?.UpdateCharacter(m_coreBoneData);
    }
}

```

## &nbsp;&nbsp;&nbsp;&nbsp;캐릭터 애니메이션 업데이트

- CoreBoneData를 통해 업데이트된 본 데이터를 Animator 본에 적용.
  
- 본의 위치(Position)와 회전(Rotation)을 실시간으로 반영하여 캐릭터 애니메이션을 동기화.

```csharp

public void UpdateCharacter(CoreBoneData coreBoneData)
{
    UpdateBone(HumanBodyBones.Hips, coreBoneData.hipPosition, coreBoneData.hipRotation);
    UpdateBone(HumanBodyBones.Spine, coreBoneData.spinePosition, coreBoneData.spineRotation);
    ...
}

private void UpdateBone(HumanBodyBones bone, float[] positionArray, float[] rotationArray)
{
    Transform boneTransform = m_animator.GetBoneTransform(bone);
    if (boneTransform == null) return;

    Vector3 position = new Vector3(positionArray[0], positionArray[1], positionArray[2]);
    Quaternion rotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

    boneTransform.position = position;
    boneTransform.rotation = rotation;
}

```

</details>

<details>
<summary> NetworkManager - 유승우 </summary>

 
### &nbsp;&nbsp;&nbsp;ICNetworkManager   
## &nbsp;&nbsp;&nbsp;&nbsp;변수 선언
    
```csharp

    // Login Info
    int UID;

    // Test InputField UI
    public InputField mIPInput, mPortInput, mNickInput;
    private ICPacketQueue SendPacketQueue;

    String mClientName;

    bool bSocketReady;
    TcpClient mSocket;
    NetworkStream mStream;
    StreamWriter mWriter;
    StreamReader mReader;

    // Thread 
    Thread sendThread;
    Thread recvThread;
    Queue<byte[]> sendQueue;
    Queue<byte[]> recvQueue;
    bool bRun = false;
    object queueLock = new object();

    // Receiver
    ICPacketReciever packetReciever;
    ICMotionReciever motionReciever;
```

## &nbsp;&nbsp;&nbsp;&nbsp; 서버 접속
```csharp
public void ConnectToServer()
{
    // if Client Connected aready, return
    if (bSocketReady) return;

    // HOST / PORT
    string ip = "58.127.66.152";
    int port = 25000;

    // Create Socket 
    try
    {
        mSocket = new TcpClient(ip, port);
        mStream = mSocket.GetStream();
        mWriter = new StreamWriter(mStream);
        mReader = new StreamReader(mStream);
        sendThread = new Thread(ProcessSendPackets);
        sendThread.Start();
        SendPacketQueue = new ICPacketQueue();

        bRun = true;
        bSocketReady = true;

        // Receiver
        motionReciever = new ICMotionReciever();
        motionReciever.Init();
    }
    catch(Exception e)
    {
        Debug.Log($"Error: Can't Create Client Socket {e}");
    }

}
```
## &nbsp;&nbsp;&nbsp;&nbsp; Queue 에 Send Packet 쌓기
```csharp  
    public void SendPacket_Bone(ICPacket_Bone packet)
    {
        int size = packet.packetHeader.nSize;
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        SendPacketQueue.Enqueue(bytes);
    }
```
## &nbsp;&nbsp;&nbsp;&nbsp; Send Packet 처리 
```csharp
    private void ProcessSendPackets()
    {
        Debug.Log("Processing thread started.");
        while (bRun)
        {

            if(SendPacketQueue == null) continue;
            byte[] dequeueBytes;
            dequeueBytes = SendPacketQueue.Dequeue();
            if (dequeueBytes != null)
            {
                // 여기서 Send
                Debug.Log("Packet");
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(ms);
                    mStream.Write(dequeueBytes, 0, dequeueBytes.Length);
                }
            }
            else
            {
                Debug.Log("Packet Empty");
            }

         
            Thread.Sleep(10);
        }

    }

```

## &nbsp;&nbsp;&nbsp;&nbsp;Convert Structure
```csharp
    private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }

```
## &nbsp;&nbsp;&nbsp;&nbsp; Recv 
```csharp
    private void ReceiveData()
    {
        try
        {
            // 헤더 크기를 읽습니다
            int headerSize = Marshal.SizeOf(typeof(StHeader));
            byte[] headerBuffer = new byte[headerSize];
            int bytesRead = mStream.Read(headerBuffer, 0, headerSize);

            if (bytesRead != headerSize)
            {
                throw new Exception("Failed to read packet header");
            }

            // 헤더 정보를 읽어 패킷 크기를 확인합니다
            StHeader header = ByteArrayToStructure<StHeader>(headerBuffer);

            // 헤더의 프로토콜에 따라 
            Parse(header);
        }
        catch (Exception e)
        {
            Debug.Log("Receive Error: " + e.Message);
        }
    }
```
## &nbsp;&nbsp;&nbsp;&nbsp; Close Socket
```csharp
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!bSocketReady) return;

        mWriter.Close();
        mReader.Close();
        mSocket.Close();
        bSocketReady = false;
    }
```
</details>

<details>
<summary> MotionSynchronizing - 유승우 </summary>

### &nbsp;&nbsp;&nbsp;ICNetworkManager   
## &nbsp;&nbsp;&nbsp;&nbsp;Parse by Packet
    
```csharp

    void Parse(StHeader header)
    {
        switch ((enProtocol)header.nID)
        {
            case enProtocol.prConnectAck:
                RecvConnectAck(header);
                break;
            case enProtocol.prBoneData:
                RecvBoneData(header);
                break;
            case enProtocol.prLoginAck:
                RecvLoginAck(header);
                break;
            case enProtocol.prMatchingAck:
                break;
            case enProtocol.prMBTI:
                RecvMBTI(header);
                break;
            case enProtocol.prAfter:
                RecvAfter(header);
                break;
            case enProtocol.prSendEmo:
                RecvEmotion(header);
                break;
            case enProtocol.prTransform:
                RecvTransformData(header);
                break;
            case enProtocol.prFirstAttract:
                RecvFisrtAttract(header);
                break;
        }
    }
```
## &nbsp;&nbsp;&nbsp;&nbsp;RecvBoneData
   
```csharp
float[] readfloat(float[] values, BinaryReader reader)
{
        values = new float[values.Length];

        for(int i = 0; i < values.Length; i++)
        {
            values[i] = reader.ReadSingle();
        }
        return values;
}

void RecvBoneData(StHeader header)
{
     int headerSize = Marshal.SizeOf(typeof(StHeader));
     int totalSize = header.nSize - headerSize;
     // 전체 패킷 데이터를 읽습니다
     byte[] buffer = new byte[totalSize];
     int bytesRead = mStream.Read(buffer, 0, totalSize);

     if (bytesRead != totalSize)
     {
         throw new Exception("Failed to read packet data");
     }

     // 데이터를 MemoryStream에 저장하고 읽어 들입니다
     using (MemoryStream ms = new MemoryStream(buffer))
     {
         BinaryReader reader = new BinaryReader(ms);

         // UID 읽기
         int UID = reader.ReadInt32();

         CoreBoneData bonepacket = new CoreBoneData();
         bonepacket.Init();

         bonepacket.headPosition = readfloat(bonepacket.headPosition, reader);
         bonepacket.headRotation = readfloat(bonepacket.headRotation, reader);
         bonepacket.neckPosition = readfloat(bonepacket.neckPosition, reader);
         bonepacket.neckRotation = readfloat(bonepacket.neckRotation, reader);
         bonepacket.chestPosition = readfloat(bonepacket.chestPosition, reader);
         bonepacket.chestRotation = readfloat(bonepacket.chestRotation, reader);
         bonepacket.spinePosition = readfloat(bonepacket.spinePosition, reader);
         bonepacket.spineRotation = readfloat(bonepacket.spineRotation, reader);
         bonepacket.hipPosition = readfloat(bonepacket.hipPosition, reader);
         bonepacket.hipRotation = readfloat(bonepacket.hipRotation, reader);
                    // Hands
         bonepacket.leftUpperArmPosition = readfloat(bonepacket.leftUpperArmPosition, reader);
         bonepacket.leftUpperArmRotation = readfloat(bonepacket.leftUpperArmRotation, reader);
         bonepacket.leftLowerArmPosition = readfloat(bonepacket.leftLowerArmPosition, reader);
         bonepacket.leftLowerArmRotation = readfloat(bonepacket.leftLowerArmRotation, reader);
         bonepacket.leftHandPosition = readfloat(bonepacket.leftHandPosition, reader);
         bonepacket.leftHandRotation = readfloat(bonepacket.leftHandRotation, reader);
         bonepacket.rightUpperArmPosition = readfloat(bonepacket.rightUpperArmPosition, reader);
         bonepacket.rightUpperArmRotation = readfloat(bonepacket.rightUpperArmRotation, reader);
         bonepacket.rightLowerArmPosition = readfloat(bonepacket.rightLowerArmPosition, reader);
         bonepacket.rightLowerArmRotation = readfloat(bonepacket.rightLowerArmRotation, reader);
         bonepacket.rightHandPosition = readfloat(bonepacket.rightHandPosition, reader);
         bonepacket.rightHandRotation = readfloat(bonepacket.rightHandRotation, reader);
                    // Foots
         bonepacket.leftFootPosition = readfloat(bonepacket.leftFootPosition, reader);
         bonepacket.leftFootRotation = readfloat(bonepacket.leftFootRotation, reader);
         bonepacket.rightFootPosition = readfloat(bonepacket.rightFootPosition, reader);
         bonepacket.rightFootRotation = readfloat(bonepacket.rightFootRotation, reader);

         motionReciever.AddDictionary(UID, bonepacket);
     }
}

```
</details>

&nbsp;

&nbsp;
# <u> 🖌️ UI 및 디자인 - 김준혁 <u>


