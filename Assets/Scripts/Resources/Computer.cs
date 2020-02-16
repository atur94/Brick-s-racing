

using System;
using Assets.Scripts;
using Assets.Scripts.Resources;
using UnityEngine;

namespace Resources
{
    public class Computer : Attachable
    {
        protected override void Init()
        {
            CanSnapToAll = true;
            allowedSnapDirections = new Vector3[]
            {
                Vector3.up,
            };
            allowedPlaceDirections = new Vector3[]
            {
                Vector3.down,
            };
            FacingDirection = Vector3.down;
            ObjectsAllowedToSnap = new Type[]
            {
                typeof(BlockBase)
            };
            ObjectsNotAllowedToSnap = new Type[]
            {
                
            };
        }
        
    }
}

