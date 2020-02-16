using System;
using Assets.Scripts;
using Assets.Scripts.Resources;
using UnityEngine;


public class Attachable : SnappableComponent
{
    public GameObject AttachedTo;

    protected override void OnBlockPlaced(GameObject parent)
    {
        BlockBase parentComponent = parent.GetComponent<BlockBase>();
        if(parentComponent != null)
        {
            parentComponent.AddAttached(this);
            AttachedTo = parent;
        }
    }

    public override void AddAttached(Attachable attachable)
    {
        if (attachable == null && Attached == null) return;
        for (int i = 0; i < Attached.Length; i++)
        {
            if (Attached[i] == null)
            {
                Attached[i] = attachable.gameObject;
                return;
            }
        }
    }

    public override bool CanBePlacedChild(GameObject parent, ref ObjectVectors vectors)
    {
        bool canBePlaced = base.CanBePlacedChild(parent, ref vectors);
        RotateTowards(parent, ref vectors);
        return canBePlaced;
    }
    public Vector3 FacingDirection = Vector3.down;
    public virtual void RotateTowards(GameObject parent , ref ObjectVectors vectors)
    {
        if (AllowedDirections != null && AllowedDirections.Length == 1)
        {
            if (vectors.currentObjectNormal != AllowedDirections[0])
            {
                // This ensures that A's down points at B's position
                Quaternion rotationOffset = Quaternion.AngleAxis(90.0f, Vector3.right);
                // Point straight towards B with secondary emphasis on the root's forward vector
                // Then, apply offset to align the downward axis instead
                transform.localRotation = Quaternion.LookRotation(transform.localPosition - parent.transform.localPosition, FacingDirection) * rotationOffset;
            }
        }
    }

}