using System;
using Assets.Scripts;
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
                var rot = Quaternion.LookRotation(parent.transform.localRotation * vectors.parentHitNormal * -1);
//                Debug.Log($"t = {vectors.parentHitNormal} t1 = {parent.transform.localRotation * Vector3.forward} , t2 = {rot}");
                transform.localRotation = rot;
            }
        }

    }
}