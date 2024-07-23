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

public class ICNetworkManager : MonoBehaviour
{
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

    //packetStruct
    public Vector3[] positions;
    public Quaternion[] rotations;

    public void ConnectToServer()
    {
        // if Client Connected aready, return
        if (bSocketReady) return;

        // HOST / PORT
        string ip = "127.0.0.1";
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
            ICPacket packetStruct = new ICPacket();
            packetStruct.MakeBone();
            packetStruct.SetMotionProtocol(Marshal.SizeOf(packetStruct));
            
            SendPacket_Bone(packetStruct);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Send("SIBAL HUCK");
        }

        if (mStream != null && mStream.DataAvailable)
        {
            ReceiveData();
        }
    }

    void Send(string data)
    {
        if (!bSocketReady) return;


        char[] chars = data.ToCharArray();
        Console.WriteLine(String.Join(", ", chars));        // a, b, c

        byte[] bytesForEncoding = Encoding.UTF8.GetBytes(data);

        mWriter.WriteLine(bytesForEncoding);
        mWriter.Flush();
    }

    void SendPacket_Bone(ICPacket packetStruct)
    {
        
        SendPacketQueue.Enqueue(packetStruct);
    }

    void writefloat(float[] values, BinaryWriter writer)
    {
        for (int i = 0; i < values.Length; i++)
        {
            writer.Write(values[i]);
        }
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
            //Debug.Log("Using Thead");

            if(SendPacketQueue == null) continue;

            ICPacket packet = SendPacketQueue.Dequeue();
            if (packet != null)
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(ms);

                    // Header First
                    // Convert UID to network byte order (big endian)
                    //writer.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(packet.packetHeader.nID)));
                    writer.Write(packet.packetHeader.nID);
                    writer.Write(packet.packetHeader.nSize);
                    writer.Write(packet.packetHeader.nType);
                    writer.Write(packet.packetHeader.nCheckSum);

                    writer.Write(packet.UID);

                    // Write positions and rotations
                    // Body
                    writefloat(packet.headPosition, writer);
                    writefloat(packet.headRotation, writer);
                    writefloat(packet.neckPosition, writer);
                    writefloat(packet.neckRotation, writer);
                    writefloat(packet.chestPosition, writer);
                    writefloat(packet.chestRotation, writer);
                    writefloat(packet.spinePosition, writer);
                    writefloat(packet.spineRotation, writer);
                    writefloat(packet.hipPosition, writer);
                    writefloat(packet.hipRotation, writer);
                    // HAND
                    writefloat(packet.leftUpperArmPosition, writer);
                    writefloat(packet.leftUpperArmRotation, writer);
                    writefloat(packet.leftLowerArmPosition, writer);
                    writefloat(packet.leftLowerArmRotation, writer);
                    writefloat(packet.leftHandPosition, writer);
                    writefloat(packet.leftHandRotation, writer);
                    writefloat(packet.rightUpperArmPosition, writer);
                    writefloat(packet.rightUpperArmRotation, writer);
                    writefloat(packet.rightLowerArmPosition, writer);
                    writefloat(packet.rightLowerArmRotation, writer);
                    writefloat(packet.rightHandPosition, writer);
                    writefloat(packet.rightHandRotation, writer);
                    // FOOT
                    writefloat(packet.leftFootPosition, writer);
                    writefloat(packet.leftFootRotation, writer);
                    writefloat(packet.rightHandPosition, writer);
                    writefloat(packet.rightHandRotation, writer);

                    byte[] data = ms.ToArray();
                    packet.SetMotionProtocol(data.Length);
                    ms.SetLength(0);

                    // Final Packet
                    writer.Write(packet.packetHeader.nID);
                    writer.Write(packet.packetHeader.nSize);
                    writer.Write(packet.packetHeader.nType);
                    writer.Write(packet.packetHeader.nCheckSum);

                    writer.Write(packet.UID);

                    // Write positions and rotations
                    // Body
                    writefloat(packet.headPosition, writer);
                    writefloat(packet.headRotation, writer);
                    writefloat(packet.neckPosition, writer);
                    writefloat(packet.neckRotation, writer);
                    writefloat(packet.chestPosition, writer);
                    writefloat(packet.chestRotation, writer);
                    writefloat(packet.spinePosition, writer);
                    writefloat(packet.spineRotation, writer);
                    writefloat(packet.hipPosition, writer);
                    writefloat(packet.hipRotation, writer);
                    // HAND
                    writefloat(packet.leftUpperArmPosition, writer);
                    writefloat(packet.leftUpperArmRotation, writer);
                    writefloat(packet.leftLowerArmPosition, writer);
                    writefloat(packet.leftLowerArmRotation, writer);
                    writefloat(packet.leftHandPosition, writer);
                    writefloat(packet.leftHandRotation, writer);
                    writefloat(packet.rightUpperArmPosition, writer);
                    writefloat(packet.rightUpperArmRotation, writer);
                    writefloat(packet.rightLowerArmPosition, writer);
                    writefloat(packet.rightLowerArmRotation, writer);
                    writefloat(packet.rightHandPosition, writer);
                    writefloat(packet.rightHandRotation, writer);
                    // FOOT
                    writefloat(packet.leftFootPosition, writer);
                    writefloat(packet.leftFootRotation, writer);
                    writefloat(packet.rightHandPosition, writer);
                    writefloat(packet.rightHandRotation, writer);

                    byte[] finalData = ms.ToArray();

                    mStream.Write(finalData, 0, finalData.Length);
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
            int totalSize = header.nSize - headerSize;

            // 전체 패킷 데이터를 읽습니다
            byte[] buffer = new byte[totalSize];
            bytesRead = mStream.Read(buffer, 0, totalSize);

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

                // positions 및 rotations 읽기
                CoreBoneData coreBoneData = new CoreBoneData();
                coreBoneData.Init();
                // Body
                coreBoneData.headPosition = readfloat(coreBoneData.headPosition, reader);
                coreBoneData.headRotation = readfloat(coreBoneData.headRotation, reader);
                coreBoneData.neckPosition = readfloat(coreBoneData.neckPosition, reader);
                coreBoneData.neckRotation = readfloat(coreBoneData.neckRotation, reader);
                coreBoneData.chestPosition = readfloat(coreBoneData.chestPosition, reader);
                coreBoneData.chestRotation = readfloat(coreBoneData.chestRotation, reader);
                coreBoneData.spinePosition = readfloat(coreBoneData.spinePosition, reader);
                coreBoneData.spineRotation = readfloat(coreBoneData.spineRotation, reader);
                coreBoneData.hipPosition = readfloat(coreBoneData.hipPosition, reader);
                coreBoneData.hipRotation = readfloat(coreBoneData.hipRotation, reader);

                // Hands
                coreBoneData.leftUpperArmPosition = readfloat(coreBoneData.leftUpperArmPosition, reader);
                coreBoneData.leftUpperArmRotation = readfloat(coreBoneData.leftUpperArmRotation, reader);
                coreBoneData.leftLowerArmPosition = readfloat(coreBoneData.leftLowerArmPosition, reader);
                coreBoneData.leftLowerArmRotation = readfloat(coreBoneData.leftLowerArmRotation, reader);
                coreBoneData.leftHandPosition = readfloat(coreBoneData.leftHandPosition, reader);
                coreBoneData.leftHandRotation = readfloat(coreBoneData.leftHandRotation, reader);
                coreBoneData.rightUpperArmPosition = readfloat(coreBoneData.rightUpperArmPosition, reader);
                coreBoneData.rightUpperArmRotation = readfloat(coreBoneData.rightUpperArmRotation, reader);
                coreBoneData.rightLowerArmPosition = readfloat(coreBoneData.rightLowerArmPosition, reader);
                coreBoneData.rightLowerArmRotation = readfloat(coreBoneData.rightLowerArmRotation, reader);
                coreBoneData.rightHandPosition = readfloat(coreBoneData.rightHandPosition, reader);
                coreBoneData.rightHandRotation = readfloat(coreBoneData.rightHandRotation, reader);

                // Foots
                coreBoneData.leftFootPosition = readfloat(coreBoneData.leftFootPosition, reader);
                coreBoneData.leftFootRotation = readfloat(coreBoneData.leftFootRotation, reader);
                coreBoneData.rightFootPosition = readfloat(coreBoneData.rightFootPosition, reader);
                coreBoneData.rightFootRotation = readfloat(coreBoneData.rightFootRotation, reader);




                float[] positions = new float[13];
                float[] rotations = new float[13];

                for (int i = 0; i < 13; i++)
                {
                    positions[i] = reader.ReadSingle();
                    rotations[i] = reader.ReadSingle();
                }


                //// 패킷 생성
                //Packet packet = new Packet
                //{
                //    packetHeader = header,
                //    UID = UID,
                //    positions = positions,
                //    rotations = rotations
                //};
                //
                //lock (packetQueue)
                //{
                //    packetQueue.Enqueue(packet);
                //}
            }
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
