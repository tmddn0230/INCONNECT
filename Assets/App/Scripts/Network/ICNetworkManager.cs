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
            ICPacket packetStruct = new ICPacket(2, positions, rotations);
            SendPacketQueue = new ICPacketQueue();

            SendPacketQueue.Enqueue(packetStruct);

            
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Send("SIBAL HUCK");
        }

        if (bSocketReady && mStream.DataAvailable)
        {
            string data = mReader.ReadLine();
            if (data != null)
            {
                Debug.Log(data);
            }
               // OnIncomingData(data);
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
                // Process 
                //Debug.Log("Send Packet!");
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    //bf.Serialize(ms, packet.packetHeader);
                //    bf.Serialize(ms, packet);
                //    byte[] data = ms.ToArray();
                //    mStream.Write(data, 0, data.Length);
                //    Debug.Log("Packet sent");
                //}

                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(ms);

                    // Convert UID to network byte order (big endian)
                    writer.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(packet.UID)));

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
                    mStream.Write(data, 0, data.Length);
                }

            }
            else
            {
                Debug.Log("Packet Empty");
            }

            Thread.Sleep(10);
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
