using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{

    public class Block
    {
        public int blockId;
        public string blockName;
        public Material blockMaterial;
        public GameObject prefab;
        public bool overrideMaterial;


        public Block(int blockId, string blockName, Material blockMaterial, GameObject prefab, bool overrideMaterial)
        {
            this.blockId = blockId;
            this.blockName = blockName;
            this.blockMaterial = blockMaterial;
            this.prefab = prefab;
            this.overrideMaterial = overrideMaterial;
        }
    }

    [Serializable]
    public struct BlockType
    {
        public string blockName;
        public Material blockMat;
        public GameObject prefab;
        public bool overrideMaterial;
    }
}
