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
    }
}