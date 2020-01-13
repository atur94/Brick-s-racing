using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public class BlockBase : MonoBehaviour, IBlockType
    {
        protected Vector3[] allowedDirections =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.back, 
            Vector3.forward, 
        };

        private void FixedUpdate()
        {
            DrawRays();
        }

        private void Start()
        {

        }

        public new void ToString()
        {
            Debug.Log($"Jestem {this.GetType()}");
        }

        public Vector3[] AllowedDirections()
        {
            return allowedDirections;
        }

        public void DrawRays()
        {
            foreach (var allowedDirection in allowedDirections)
            {
                Debug.DrawLine(transform.position, transform.position + allowedDirection*3, Color.yellow, 0.01f);
            }
        }
    }
}
