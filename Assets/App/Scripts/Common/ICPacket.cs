using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace INCONNECT.Packet
{
    [System.Serializable]
    public class ICPacket 
    {
        int UID;
        public Vector3[] positions { get; private set; }
        public Quaternion[] rotations { get; private set; }

        public ICPacket(Vector3[] vec, Quaternion[] quat)
        {
            positions = vec;
            rotations = quat;
        }
    }
}


