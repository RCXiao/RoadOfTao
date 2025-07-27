using Unity.Mathematics;
using UnityEngine;

public struct CardTransform
{
    public Vector3 pos;
    public quaternion rot;

    public CardTransform(Vector3 vector3, quaternion quaternion)
    {
        pos = vector3;
        rot = quaternion;
    }
}
