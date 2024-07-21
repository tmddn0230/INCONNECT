﻿using System;
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

            //CoreBoneData testDatas;
            //testDatas = new CoreBoneData();
            ////Head
            //testDatas.headPosition = new float[] {1.0f, 2.0f, 3.0f };
            

            // Dummy Vec, Rot
            positions = new Vector3[3];
            Vector3 vector0 = new Vector3(1, 0, 0);
            Vector3 vector1 = new Vector3(1, 2, 0);
            Vector3 vector2 = new Vector3(1, 2, 3);

            positions[0] = vector0;
            positions[1] = vector1;
            positions[2] = vector2;

            rotations = new Quaternion[3];
            Quaternion quat0 = new Quaternion(1, 0, 0, 1);
            Quaternion quat1 = new Quaternion(1, 2, 0, 1);
            Quaternion quat2 = new Quaternion(1, 2, 3, 1);

            rotations[0] = quat0;
            rotations[1] = quat1;
            rotations[2] = quat2;


            // Queue Init
            ICPacket packetStruct = new ICPacket();
            packetStruct.SetMotionProtocol(Marshal.SizeOf(packetStruct));

            SendPacketQueue = new ICPacketQueue();
            SendPacketQueue.Enqueue(packetStruct);

            
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
                    //writer.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(packet.packetHeader.nID)));
                    writer.Write(packet.packetHeader.nID);
                    writer.Write(packet.packetHeader.nSize);
                    writer.Write(packet.packetHeader.nType);
                    writer.Write(packet.packetHeader.nCheckSum);
                    // Convert UID to network byte order (big endian)
                    writer.Write(packet.UID);

                    // Write positions and rotations
                    foreach (float position in packet.positions)
                    {
                        writer.Write(position);
                    }
                    foreach (float rotation in packet.rotations)
                    {
                        writer.Write(rotation);
                    }

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
                    foreach (float position in packet.positions)
                    {
                        writer.Write(position);
                    }
                    foreach (float rotation in packet.rotations)
                    {
                        writer.Write(rotation);
                    }

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
                long UID = reader.ReadInt64();

                // positions 및 rotations 읽기
                int numElements = (totalSize - sizeof(long)) / (sizeof(float) * 2);
                float[] positions = new float[numElements];
                float[] rotations = new float[numElements];

                for (int i = 0; i < numElements; i++)
                {
                    positions[i] = reader.ReadSingle();
                }

                for (int i = 0; i < numElements; i++)
                {
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
