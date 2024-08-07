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
        prTransform,
        prMatchingReq, prMatchingAck,
        prFirstAttract,
        prMBTI,
        prSendEmo,
        prAfter,

        PROTOCOL_END
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]   // Pack �� ����� ���İ����� 1����Ʈ  ����� ���� 1 ����Ʈ, 4����Ʈ ��� ����ü�� ũ��� 5����Ʈ�� ���
                                                      // �������� ������ 8����Ʈ
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
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // ����� ���� ����
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
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // ����� ���� ����
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
            size = Marshal.SizeOf(packetHeader) + sizeof(int) + sizeof(int); // ����� ���� ����
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
            size = Marshal.SizeOf(packetHeader) + sizeof(int); // ����� ���� ����
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
            size = Marshal.SizeOf(packetHeader) + sizeof(int); // ����� ���� ����
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


