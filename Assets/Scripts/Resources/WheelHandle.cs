using System;
using Assets.Scripts.Resources;
using UnityEngine;

public class WheelHandle : SnappableComponent
{
    protected override void Init()
    {
        CanSnapToAll = true;

        AllowedDirections = new Vector3[]
        {
            Vector3.down,
            Vector3.up,
            Vector3.back,
            Vector3.left,
            Vector3.right,
        };
        SnapDirections = new Vector3[]
        {
            Vector3.forward
        };

        ObjectsAllowedToSnap = new Type[]{typeof(Wheel)};

        base.Init();
    }



}
