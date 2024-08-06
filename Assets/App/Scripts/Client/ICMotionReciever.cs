using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Net
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Packet;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using UnityEditor.Sprites;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System.Xml;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Analytics;

public class ICMotionReciever
{
    private ICPacketQueue RecvPacketQueue;

    Thread recvThread;

    bool bRun = false;
    object queueLock = new object();

    // Dic 방식
    Dictionary<int, CoreBoneData> MotionDic = new Dictionary<int, CoreBoneData>();

    public void Init()
    {
        recvThread = new Thread(ProcessRecvPackets);
        recvThread.Start();
        RecvPacketQueue = new ICPacketQueue();

        bRun = true;
    }

    public void AddDictionary(int uid, CoreBoneData core)
    {
        MotionDic.Add(uid, core);
    }

    void ProcessRecvPackets()
    {
        /*
        Debug.Log("Processing thread started.");
        while (bRun)
        {
            //Debug.Log("Using Thead");

            if (RecvPacketQueue == null) continue;

            ICPacket_Bone packet = RecvPacketQueue.Dequeue();
            if (packet != null)
            {
                // positions 및 rotations 읽기
                CoreBoneData coreBoneData = new CoreBoneData();
                coreBoneData.Init();
                // Body
                coreBoneData.headPosition = packet.headPosition;
                coreBoneData.headRotation = packet.headRotation;
                coreBoneData.neckPosition = packet.neckPosition;
                coreBoneData.neckRotation = packet.neckRotation;
                coreBoneData.chestPosition = packet.chestPosition;
                coreBoneData.chestRotation = packet.chestRotation;
                coreBoneData.spinePosition = packet.spinePosition;
                coreBoneData.spineRotation = packet.spineRotation;
                coreBoneData.hipPosition = packet.hipPosition;
                coreBoneData.hipRotation = packet.hipRotation;

                // Hands
                coreBoneData.leftUpperArmPosition = packet.leftUpperArmPosition;
                coreBoneData.leftUpperArmRotation = packet.leftUpperArmRotation;
                coreBoneData.leftLowerArmPosition = packet.leftLowerArmPosition;
                coreBoneData.leftLowerArmRotation = packet.leftLowerArmRotation;
                coreBoneData.leftHandPosition = packet.leftHandPosition;
                coreBoneData.leftHandRotation = packet.leftHandRotation;
                coreBoneData.rightUpperArmPosition = packet.rightUpperArmPosition;
                coreBoneData.rightUpperArmRotation = packet.rightUpperArmRotation;
                coreBoneData.rightLowerArmPosition = packet.rightLowerArmPosition;
                coreBoneData.rightLowerArmRotation = packet.rightLowerArmRotation;
                coreBoneData.rightHandPosition = packet.rightHandPosition;
                coreBoneData.rightHandRotation = packet.rightHandRotation;

                // Foots
                coreBoneData.leftFootPosition = packet.leftFootPosition;
                coreBoneData.leftFootRotation = packet.leftFootRotation;
                coreBoneData.rightFootPosition = packet.rightFootPosition;
                coreBoneData.rightFootRotation = packet.rightFootRotation;
            }
            else
            {
                Debug.Log("Packet Empty");
            }

            Thread.Sleep(10);
        }
        */
    }

    public void AddPacketQueue(ICPacket_Bone packet)
    {
       // RecvPacketQueue.Enqueue(packet);
    }

    void Parse(Int32 UID)
    {
        // UID 로 캐릭터를 구분하여 bone data 를 적용할 수 있게 해야함
    }

}
