using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorManager
{
      // 캐릭터 Data 구조체
    // 기존 구조체가 너무 긴 관계로 일단 가장 중요한 Bone만 받아오는걸로
    // Hip(엉덩이), Spine(척추), Chest(가슴), Neck(목), Head(머리), Foot(발)
    // 손은 컨트롤러 또는 핸드트래킹으로 대체 예정이라 일단 제외하였음.

    public struct ActorData
    {
        public string name;
        public float[] hipPosition;
        public float[] hipRotation;
        public float[] spinePosition;
        public float[] spineRotation;
        public float[] chestPosition;
        public float[] chestRotation;
        public float[] neckPosition;
        public float[] neckRotation;
        public float[] headPosition;
        public float[] headRotation;
        public float[] leftFootPosition;
        public float[] leftFootRotation;
        public float[] rightFootPosition;
        public float[] rightFootRotation;
    }

}
