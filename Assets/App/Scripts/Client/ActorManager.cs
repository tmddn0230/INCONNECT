using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorManager
{
      // ĳ���� Data ����ü
    // ���� ����ü�� �ʹ� �� ����� �ϴ� ���� �߿��� Bone�� �޾ƿ��°ɷ�
    // Hip(������), Spine(ô��), Chest(����), Neck(��), Head(�Ӹ�), Foot(��)
    // ���� ��Ʈ�ѷ� �Ǵ� �ڵ�Ʈ��ŷ���� ��ü �����̶� �ϴ� �����Ͽ���.

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
