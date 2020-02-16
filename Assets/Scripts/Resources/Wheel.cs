using Assets.Scripts.Resources;
using UnityEngine;

public class Wheel : Attachable
{
    protected override void Init()
    {
        allowedSnapDirections = null;
        allowedPlaceDirections = new Vector3[]
        {
            Vector3.forward,
        };
//        ObjectsAllowedToSnap = null;
//        ObjectsNotAllowedToSnap = null;

    }

    public override void RotateTowards(GameObject parent, ref ObjectVectors vectors)
    {
        if (AllowedDirections != null && AllowedDirections.Length == 1)
        {
            if (vectors.currentObjectNormal != AllowedDirections[0])
            {
                var rot = Quaternion.LookRotation(parent.transform.localRotation * vectors.parentHitNormal * -1);
                transform.localRotation = rot;
            }
        }
    }
}