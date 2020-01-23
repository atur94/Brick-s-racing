using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public class PrismCorner : BlockBase
    {
        protected override void Init()
        {
            allowedDirections = new Vector3[]
            {
                Vector3.right,
                Vector3.forward,
                Vector3.down
            };
        }
    }
}
