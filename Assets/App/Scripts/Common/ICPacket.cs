using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


namespace Packet
{
    public enum enProtocol
    {
        PROTOCOL_START = 0,
        prConnectAck,
        prLoginReq, prLoginAck,
        prBoneData,
        prENTPORTAL,

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

        // Dummy Make Bone Func, 제거예정
        public void MakeBone()
        {
           // headPosition = new float[] {0, 0, 0};
           // headRotation = new float[] {0, 0, 0, 1 };
           // neckPosition = new float[] { 0, 0, 1 };
           // neckRotation = new float[] { 0, 0, 1, 1 };
           // chestPosition = new float[] { 0, 1, 1 };
           // chestRotation = new float[] { 0, 1, 1, 1 };
           // spinePosition = new float[] { 1, 1, 1 };
           // spineRotation = new float[] { 1, 1, 1, 1 };
           // hipPosition = new float[] { 0, 0, -1 };
           // hipRotation = new float[] { 0, 0, -1, 1 };
           //
           //
           //
           // leftUpperArmPosition = new float[] { 0, 1, -1 };
           // leftUpperArmRotation = new float[] { 0, 1, -1, 1 };
           // leftLowerArmPosition = new float[] { 1, 1, -1 };
           // leftLowerArmRotation = new float[] { 1, 1, -1, 1 };
           // leftHandPosition = new float[] { 0, -1, -1 };
           // leftHandRotation = new float[] { 0, -1, -1, 1 };
           // rightUpperArmPosition = new float[] { 1, -1, -1 };
           // rightUpperArmRotation = new float[] { -1, -1, -1, 1 };
           // rightLowerArmPosition = new float[] { -1, -1, -1 };
           // rightLowerArmRotation = new float[] { 0, 0, 2, 1 };
           // rightHandPosition = new float[] { 0, 0, 2 };
           // rightHandRotation = new float[] { 0, 1, 2, 1 };
           //
           //
           // leftFootPosition = new float[] { 0, 1, 2 };
           // leftFootRotation = new float[] { 1, 1, 2, 1 };
           // rightFootPosition = new float[] { 1, 1, 2 };
           // rightFootRotation = new float[] { 1, 2, 2, 1 };
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


