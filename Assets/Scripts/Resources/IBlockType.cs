
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public interface IBlockType
    {
        void ToString();

        Vector3[] AllowedDirections { get; set; }
        void AddConnectedBlock(GameObject joinedBlock);
        void AddJoinedBlocks(GameObject[] joinedBlocks);
        void Rotate(Vector3 axis, float angle, Space relativeTo);
        void DeleteBlock();

    }




}
