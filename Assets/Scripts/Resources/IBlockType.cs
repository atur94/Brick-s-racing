
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public interface IBlockType
    {
        void ToString();

        Vector3[] AllowedDirections();
    }



}
