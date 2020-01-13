using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public class Prism : BlockBase
    {
        Prism()
        {

            Debug.Log("Prism creating");

            allowedDirections = new Vector3[]
            {
                Vector3.left, Vector3.right, Vector3.down, Vector3.forward, Vector3.back, 
            };

            Debug.Log("Prism created");
        }
    }
}
