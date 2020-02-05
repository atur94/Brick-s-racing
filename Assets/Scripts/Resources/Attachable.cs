using System;
using Assets.Scripts.Resources;
using UnityEngine;


public class Attachable : SnappableComponent
{
    public GameObject AttachedTo;

    public override void OnBlockPlaced(GameObject parent)
    {
        AttachedTo = parent;
    }

    public override bool CanBePlacedChild(GameObject parent, ref ObjectVectors vectors)
    {
        bool canBePlaced = base.CanBePlacedChild(parent, ref vectors);
        RotateTowards(parent, ref vectors);
        return canBePlaced;
    }

    private void RotateTowards(GameObject parent , ref ObjectVectors vectors)
    {
        if (AllowedDirections != null && AllowedDirections.Length == 1)
        {
            if (vectors.currentObjectNormal != AllowedDirections[0])
            {
                Debug.Log($"{vectors.parentHitNormal}");
                var rot = Quaternion.FromToRotation(AllowedDirections[0] * -1, vectors.parentHitNormal * -1);
                transform.localRotation = rot;
            }
        }

    }
}