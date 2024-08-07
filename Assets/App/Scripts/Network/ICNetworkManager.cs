using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Net
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Packet;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using UnityEditor.Sprites;
using Unity.VisualScripting;
using Newtonsoft.Json;
using UnityEngine.Analytics;
using System.Drawing;
using UnityEngine.SocialPlatforms.Impl;

public class ICNetworkManager : MonoBehaviour
{
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
    public ICMotionReciever GetReciever() { return motionReciever; }
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

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Queue Init
            ICPacket_Bone packetStruct = new ICPacket_Bone();
            //packetStruct.MakeBone();
            packetStruct.SetMotionProtocol();
            
            SendPacket_Bone(packetStruct);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Login("","");
        }

        if (mStream != null && mStream.DataAvailable)
        {
            ReceiveData();
        }
    }

    void Login(string ID, string Password)
    {

        // Queue Init
        ICPacket_Login packetStruct = new ICPacket_Login();
        //packetStruct.MakeBone();
        packetStruct.SetLoginProtocol();

        SendPacket_Login(packetStruct);
    }



    void TestSend(string data)
    {
        if (!bSocketReady) return;


        char[] chars = data.ToCharArray();
        Console.WriteLine(String.Join(", ", chars));        // a, b, c

        byte[] bytesForEncoding = Encoding.UTF8.GetBytes(data);

        mWriter.WriteLine(bytesForEncoding);
        mWriter.Flush();
    }

    

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

    void SendPacket_Attract(int score)
    {
        ICPacket_First packetStruct = new ICPacket_First();
        packetStruct.SetAttractProtocol();
        packetStruct.Score = score;

        int size = packetStruct.packetHeader.nSize;
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(packetStruct, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        SendPacketQueue.Enqueue(bytes);
    }

    void SendPacket_Login(ICPacket_Login packet)
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

    void SendPacket_After(int result)
    {
        ICPacket_After packetStruct = new ICPacket_After();
        packetStruct.SetAfterProtocol();
        packetStruct.Result = result;

        int size = packetStruct.packetHeader.nSize;
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(packetStruct, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        SendPacketQueue.Enqueue(bytes);
    }

    void SendPacket_Transform(ICPacket_Transform packet)
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

    void SendPacket_MBTI(int mbti)
    {
        ICPacket_MBTI packetStruct = new ICPacket_MBTI();
        packetStruct.SetMBTIProtocol();
        packetStruct.MBTI = mbti;

        int size = packetStruct.packetHeader.nSize;
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(packetStruct, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        SendPacketQueue.Enqueue(bytes);
    }

    void SendPacket_EMO(int emotion)
    {
        ICPacket_EMO packetStruct = new ICPacket_EMO();
        packetStruct.SetEmoProtocol();
        packetStruct.EMO = emotion;

        int size = packetStruct.packetHeader.nSize;
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(packetStruct, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        SendPacketQueue.Enqueue(bytes);
    }

    float[] readfloat(float[] values, BinaryReader reader)
    {
        values = new float[values.Length];

        for(int i = 0; i < values.Length; i++)
        {
            values[i] = reader.ReadSingle();
        }
        return values;
    }


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

    private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }

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

    void RecvConnectAck(StHeader header)
    {
        int headerSize = Marshal.SizeOf(typeof(StHeader));
        int totalSize = header.nSize - headerSize;
        // 전체 패킷 데이터를 읽습니다
        int bytesRead;
        byte[] buffer = new byte[totalSize];
        bytesRead = mStream.Read(buffer, 0, totalSize);

        if (bytesRead != totalSize)
        {
            throw new Exception("Failed to read packet data");
        }

        // 데이터를 MemoryStream에 저장하고 읽어 들입니다
        using (MemoryStream ms = new MemoryStream(buffer))
        {

        }
    }
    void RecvLoginAck(StHeader header)
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
            // Result 읽기
            int Result = reader.ReadInt32();

            ICClient.Instance.Actor_Spawn(UID, Result);
        }
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


            //ICPacket_Bone recvBoneData;
            //recvBoneData.packetHeader = header;
            //recvBoneData.UID = UID;
            //recvBoneData.bonedata = bonepacket;

            //motionReciever.AddPacketQueue(recvBoneData);
            motionReciever.AddDictionary(UID, bonepacket);
        }
    }

    void RecvTransformData(StHeader header)
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
        }
    }

    void RecvMatchAck(StHeader header)
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

            
        }
    }
    void RecvFisrtAttract(StHeader header)
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
            // Score 읽기
            int Score = reader.ReadInt32();
        }
    }

    void RecvMBTI(StHeader header)
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
            // MBTI 읽기
            int MBTI = reader.ReadInt32();
        }
    }
    void RecvEmotion(StHeader header)
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
            // EMO 읽기
            int EMO = reader.ReadInt32();
        }
    }

    void RecvAfter(StHeader header)
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
            // Result 읽기
            int Result = reader.ReadInt32();
        }
    }

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


}
