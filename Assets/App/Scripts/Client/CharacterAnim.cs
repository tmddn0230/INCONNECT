using Rokoko.Core;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{
    public Transform hip;
    public Transform spine;
    public Transform chest;
    public Transform neck;
    public Transform head;
    public Transform leftFoot;
    public Transform rightFoot;

    public void ApplyCoreBoneData(CoreBoneData coreBoneData)
    {
        ApplyTransform(hip, coreBoneData.hipPosition, coreBoneData.hipRotation);
        ApplyTransform(spine, coreBoneData.spinePosition, coreBoneData.spineRotation);
        ApplyTransform(chest, coreBoneData.chestPosition, coreBoneData.chestRotation);
        ApplyTransform(neck, coreBoneData.neckPosition, coreBoneData.neckRotation);
        ApplyTransform(head, coreBoneData.headPosition, coreBoneData.headRotation);
        ApplyTransform(leftFoot, coreBoneData.leftFootPosition, coreBoneData.leftFootRotation);
        ApplyTransform(rightFoot, coreBoneData.rightFootPosition, coreBoneData.rightFootRotation);
    }

    private void ApplyTransform(Transform target, float[] position, float[] rotation)
    {
        if (target != null)
        {
            target.localPosition = new Vector3(position[0], position[1], position[2]);
            target.localRotation = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
        }
    }
}
