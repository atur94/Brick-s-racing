
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public interface IBlockType
    {
        void ToString();

        Vector3[] AllowedDirections();
        void AddConnectedBlock(GameObject joinedBlock);
        void AddJoinedBlocks(GameObject[] joinedBlocks);
        void Rotate(Vector3 axis, float angle, Space relativeTo);
        void TestForNeighbours();
        void DeleteBlock();
        void BlockPlaced();

    }




}
