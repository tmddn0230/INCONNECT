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

&nbsp;
&nbsp;

[Client] : C#
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


