using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


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



    [System.Serializable]
    public class ICPacket 
    {

        public int UID;
        public float[] positions;
        public float[] rotations;

        public ICPacket(int uid, Vector3[] vec, Quaternion[] quat)
        {
            UID = uid;
            positions = new float[vec.Length * 3];
            rotations = new float[quat.Length * 4];

            for (int i = 0; i < vec.Length; i++)
            {
                positions[i * 3] = vec[i].x;
                positions[i * 3 + 1] = vec[i].y;
                positions[i * 3 + 2] = vec[i].z;
            }

            for (int i = 0; i < quat.Length; i++)
            {
                rotations[i * 4] = quat[i].x;
                rotations[i * 4 + 1] = quat[i].y;
                rotations[i * 4 + 2] = quat[i].z;
                rotations[i * 4 + 3] = quat[i].w;
            }
        }

        public Vector3[] GetPositions()
        {
            Vector3[] vec = new Vector3[positions.Length / 3];
            for (int i = 0; i < positions.Length; i++)
            {
                vec[i] = new Vector3(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
            }
            return vec;
        }

        public Quaternion[] GetRotations()
        {
            Quaternion[] quat = new Quaternion[rotations.Length / 4];
            for (int i = 0; i < rotations.Length; i++)
            {
                quat[i] = new Quaternion(rotations[i * 4], rotations[i * 4 + 1], rotations[i * 4 + 2], rotations[i * 4 + 3]);
            }
            return quat;
        }

    }

    public class ICPacketQueue
    {
        private Queue<ICPacket> queue;
        
        public ICPacketQueue()
        {
            queue = new Queue<ICPacket>();
        }

        public void Enqueue(ICPacket packet)
        {
            queue.Enqueue (packet);
            Debug.Log("Enqueue !");
        }

        public ICPacket Dequeue()
        {
            if (queue.Count > 0)
            {
                ICPacket packet = queue.Dequeue();
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


