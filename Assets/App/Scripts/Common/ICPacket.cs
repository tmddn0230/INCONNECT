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


    [StructLayout(LayoutKind.Sequential)]
    [System.Serializable]
    public class ICPacket 
    {
        public StHeader packetHeader;

        public int UID;

        // Body : 5
        public float[] headPosition = new float[3];
        public float[] headRotation = new float[4];
        public float[] neckPosition = new float[3];
        public float[] neckRotation = new float[4];
        public float[] chestPosition = new float[3];
        public float[] chestRotation = new float[4];
        public float[] spinePosition = new float[3];
        public float[] spineRotation = new float[4];
        public float[] hipPosition = new float[3];
        public float[] hipRotation = new float[4];


        // Hand : 6
        public float[] leftUpperArmPosition = new float[3];
        public float[] leftUpperArmRotation = new float[4];
        public float[] leftLowerArmPosition = new float[3];
        public float[] leftLowerArmRotation = new float[4];
        public float[] leftHandPosition = new float[3];
        public float[] leftHandRotation = new float[4];
        public float[] rightUpperArmPosition = new float[3];
        public float[] rightUpperArmRotation = new float[4];
        public float[] rightLowerArmPosition = new float[3];
        public float[] rightLowerArmRotation = new float[4];
        public float[] rightHandPosition = new float[3];
        public float[] rightHandRotation = new float[4];

        // Leg : 2
        public float[] leftFootPosition = new float[3];
        public float[] leftFootRotation = new float[4];
        public float[] rightFootPosition = new float[3];
        public float[] rightFootRotation = new float[4];

 
        public void SetMotionProtocol(int len)
        {
            packetHeader = new StHeader();
            packetHeader.SetHeader((int)enProtocol.prBoneData, len);
        }

        // Dummy Make Bone Func

        public void MakeBone()
        {
            headPosition = new float[] {0, 0, 0};
            headRotation = new float[] {0, 0, 0, 1 };
            neckPosition = new float[] { 0, 0, 1 };
            neckRotation = new float[] { 0, 0, 1, 1 };
            chestPosition = new float[] { 0, 1, 1 };
            chestRotation = new float[] { 0, 1, 1, 1 };
            spinePosition = new float[] { 1, 1, 1 };
            spineRotation = new float[] { 1, 1, 1, 1 };
            hipPosition = new float[] { 0, 0, -1 };
            hipRotation = new float[] { 0, 0, -1, 1 };



            leftUpperArmPosition = new float[] { 0, 1, -1 };
            leftUpperArmRotation = new float[] { 0, 1, -1, 1 };
            leftLowerArmPosition = new float[] { 1, 1, -1 };
            leftLowerArmRotation = new float[] { 1, 1, -1, 1 };
            leftHandPosition = new float[] { 0, -1, -1 };
            leftHandRotation = new float[] { 0, -1, -1, 1 };
            rightUpperArmPosition = new float[] { 1, -1, -1 };
            rightUpperArmRotation = new float[] { -1, -1, -1, 1 };
            rightLowerArmPosition = new float[] { -1, -1, -1 };
            rightLowerArmRotation = new float[] { 0, 0, 2, 1 };
            rightHandPosition = new float[] { 0, 0, 2 };
            rightHandRotation = new float[] { 0, 1, 2, 1 };


            leftFootPosition = new float[] { 0, 1, 2 };
            leftFootRotation = new float[] { 1, 1, 2, 1 };
            rightFootPosition = new float[] { 1, 1, 2 };
            rightFootRotation = new float[] { 1, 2, 2, 1 };
        }


        // 제거예정
        //public ICPacket(int uid, Vector3[] vec, Quaternion[] quat)
        //{
        //    UID = uid;
        //    positions = new float[vec.Length * 3];
        //    rotations = new float[quat.Length * 4];
        //
        //    for (int i = 0; i < vec.Length; i++)
        //    {
        //        positions[i * 3] = vec[i].x;
        //        positions[i * 3 + 1] = vec[i].y;
        //        positions[i * 3 + 2] = vec[i].z;
        //    }
        //
        //    for (int i = 0; i < quat.Length; i++)
        //    {
        //        rotations[i * 4] = quat[i].x;
        //        rotations[i * 4 + 1] = quat[i].y;
        //        rotations[i * 4 + 2] = quat[i].z;
        //        rotations[i * 4 + 3] = quat[i].w;
        //    }
        //}
        //
        //public ICPacket(int uid, float[] pos, float[] quat)
        //{
        //    UID = uid;
        //    for (int i = 0; i < pos.Length; i++)
        //    {
        //        positions[i] = pos[i]; 
        //    }
        //
        //    for (int j = 0; j < quat.Length; j++)
        //    {
        //        rotations[j] = quat[j];
        //    }
        //}

        //public Vector3[] GetPositions()
        //{
        //    Vector3[] vec = new Vector3[positions.Length / 3];
        //    for (int i = 0; i < positions.Length; i++)
        //    {
        //        vec[i] = new Vector3(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
        //    }
        //    return vec;
        //}
        //
        //public void SetPositions(Vector3[] vec)
        //{
        //    for (int i = 0; i < vec.Length; i++)
        //    {
        //        positions[i * 3] = vec[i].x;
        //        positions[i * 3 + 1] = vec[i].y;
        //        positions[i * 3 + 2] = vec[i].z;
        //    }
        //}
        //
        //
        //public Quaternion[] GetRotations()
        //{
        //    Quaternion[] quat = new Quaternion[rotations.Length / 4];
        //    for (int i = 0; i < rotations.Length; i++)
        //    {
        //        quat[i] = new Quaternion(rotations[i * 4], rotations[i * 4 + 1], rotations[i * 4 + 2], rotations[i * 4 + 3]);
        //    }
        //    return quat;
        //}
        //
        //public void SetRotations(Quaternion[] quat)
        //{
        //    for (int i = 0; i < quat.Length; i++)
        //    {
        //        rotations[i * 4] = quat[i].x;
        //        rotations[i * 4 + 1] = quat[i].y;
        //        rotations[i * 4 + 2] = quat[i].z;
        //        rotations[i * 4 + 3] = quat[i].w;
        //    }
        //}
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


