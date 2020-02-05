
using UnityEngine;

namespace Assets.Scripts.Resources
{

    public class Cube : BlockBase
    {
        protected override void Init()
        {
            allowedPlaceDirections = new Vector3[]
            {
                Vector3.down,
                Vector3.up,
                Vector3.left,
                Vector3.right,
                Vector3.back, 
                Vector3.forward, 
            };
        }
    }
}
