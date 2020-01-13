using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    [SerializeField] private BlockType[] allBlockTypes;

    [HideInInspector] public Dictionary<int, Block> allBlocks = new Dictionary<int, Block>();

    private void Awake()
    {
        for (int i = 0; i < allBlockTypes.Length; i++)
        {
            BlockType newBlockType = allBlockTypes[i];
            Block newBlock = new Block(i, newBlockType.blockName, newBlockType.blockMat, newBlockType.prefab);
            allBlocks[i] = newBlock;
            print($"Block {allBlocks[i].blockName} added to dictionary ");
        }
    }
}
