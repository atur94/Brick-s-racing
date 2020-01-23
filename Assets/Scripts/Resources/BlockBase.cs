using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Resources
{
    public abstract class BlockBase : MonoBehaviour, IBlockType
    {
        [SerializeField]
        protected GameObject[] connectedBlocks;


        protected Vector3[] allowedDirections =
        {
            Vector3.down,
            Vector3.up,
            Vector3.left,
            Vector3.right,
            Vector3.back, 
            Vector3.forward, 
        };

        public Vector3[] rotatedDirections;

        protected uint Health = 100;

        [HideInInspector]
        public bool isKinematic = false; 

        private void Awake()
        {
            Init();
            connectedBlocks = new GameObject[allowedDirections.Length];
        }

        protected abstract void Init();

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if(rb == null) print("Rigidbody for this block is null");
            rb.isKinematic = isKinematic;
            rotatedDirections = new Vector3[allowedDirections.Length];
            for (int i = 0; i < allowedDirections.Length; i++)
            {
                rotatedDirections[i] = transform.rotation * allowedDirections[i];
            }
        }

        public new void ToString()
        {
            Debug.Log($"Jestem {this.GetType()}");
        }

        public Vector3[] AllowedDirections()
        {
            return allowedDirections;
        }

        public bool CheckIfConnectedBlockExist(GameObject joinedBlock)
        {
            if (joinedBlock is null) throw new NullReferenceException("Passed game object is null");
            for (int i = 0; i < connectedBlocks.Length; i++)
            {
                if (connectedBlocks[i] == joinedBlock) return true;
            }

            return false;
        }

        public void AddConnectedBlock(GameObject joinedBlock)
        {
            if (connectedBlocks != null)
            {
                if (CheckIfConnectedBlockExist(joinedBlock)) return;
                for (var i = 0; i < connectedBlocks.Length; i++)
                {
                    var connectedBlockArr = connectedBlocks[i];
                    if (connectedBlockArr == null)
                    {
                        connectedBlocks[i] = joinedBlock;
                        Debug.Log("Block added");
                        break;
                    }
                }
            }
        }

        public void AddJoinedBlocks(GameObject[] joinedBlocks)
        {
            if (joinedBlocks == null) joinedBlocks = new GameObject[6];
        }

        public void Rotate(Vector3 axis, float angle, Space relativeTo)
        {
            transform.Rotate(axis, angle, relativeTo);
            rotatedDirections = new Vector3[allowedDirections.Length];
            for (int i = 0; i < allowedDirections.Length; i++)
            {
                rotatedDirections[i] = transform.rotation * allowedDirections[i];
            }
        }


        public void DrawRays()
        {
            foreach (var allowedDirection in allowedDirections)
            {
                Debug.DrawLine(transform.position, transform.position + allowedDirection*3, Color.yellow, 0.01f);
            }
        }

        public void TestForNeighbours()
        {
            connectedBlocks = new GameObject[allowedDirections.Length];
            int elements = 0;
            Vector3[] rotatedVectors = new Vector3[allowedDirections.Length];
            RaycastHit info;
            Ray ray;
            for (var index = 0; index < connectedBlocks.Length; index++)
            {
                rotatedVectors[index] = transform.rotation * allowedDirections[index];
                Debug.DrawRay(transform.position, rotatedVectors[index], Color.magenta, 3f);
                ray = new Ray(transform.position, rotatedVectors[index]);

                if (Physics.Raycast(ray, out info, 0.55f, LayerList.cubesLayer | LayerList.groundLayer))
                {
                    var collidingObject = info.collider.gameObject.GetComponent<BlockBase>();
                    if (collidingObject != null)
                    {
                        for (int i = 0; i < collidingObject.allowedDirections.Length; i++)
                        {
                            if (collidingObject.rotatedDirections[i] * -1 == rotatedDirections[index])
                            {
                                connectedBlocks[index] = info.collider.gameObject;
                                elements++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        connectedBlocks[index] = info.collider.gameObject;
                        elements++;
                    }

                    //                    for (int i = 0; i < connectedBlocks.Length; i++)
                    //                    {
                    //                        if (connectedBlocks[i] == null)
                    //                        {
                    //                            connectedBlocks[i] = info.collider.gameObject;
                    //                            elements++;
                    //                            break;
                    //                        }
                    //                    }
                }
            }
        }

        

        IEnumerator NeighbourTestCoroutine()
        {
            yield return new WaitForFixedUpdate();
            rotatedDirections = new Vector3[allowedDirections.Length];
            for (int i = 0; i < allowedDirections.Length; i++)
            {
                rotatedDirections[i] = transform.localRotation * allowedDirections[i];
            }
            for (var i = 0; i < connectedBlocks.Length; i++)
            {
                
                var connectedGameObject = connectedBlocks[i];
                if (connectedGameObject == null) continue;
                BlockBase block = connectedGameObject.GetComponent<BlockBase>();
                if (block != null)
                {
                    block.TestForNeighbours();
//                    CheckIfObjectsExistInEachOther(this, block);
                }
                //Check if exist in both objects

            }

        }

        public void CheckIfObjectsExistInEachOther(BlockBase first, BlockBase second)
        {
            int? indexFirst = null;
            int? indexSecond = null;
            GameObject first1 = null;
            GameObject second2 = null;
            for (var i = 0; i < first.connectedBlocks.Length; i++)
            {
                var firsts = first.connectedBlocks[i];
                if(firsts == null) continue;
                if (firsts.gameObject == second.gameObject)
                {
                    indexSecond = i;
                    first1 = firsts;
                    break;
                }
            }

            for (int i = 0; i < second.connectedBlocks.Length; i++)
            {
                var seconds = second.connectedBlocks[i];
                if(seconds == null)continue;
                if (seconds.gameObject == first.gameObject)
                {
                    indexFirst = i;
                    second2 = seconds;
                    break;
                }
            }

            if (indexFirst != null && indexSecond != null)
            {
                print("Obiekty istnieja");
                return;
            }

            if (first1 != null)
            {
                first1 = null;
            }

            if (second2 != null) second2 = null;
        }

        public void BlockPlaced()
        {
            rotatedDirections = new Vector3[allowedDirections.Length];
            for (int i = 0; i < allowedDirections.Length; i++)
            {
                rotatedDirections[i] = transform.localRotation * allowedDirections[i];
            }
            TestForNeighbours();
            StartCoroutine(NeighbourTestCoroutine());
//            for (var i = 0; i < connectedBlocks.Length; i++)
//            {
//                var connectedGameObject = connectedBlocks[i];
//                if(connectedGameObject == null) continue;
//                BlockBase block = connectedGameObject.GetComponent<BlockBase>();
//                if (block != null)
//                {
//                    block.TestForNeighbours();
//                    var b = block.connectedBlocks;
//                    var name = block.name;
//                }
//            }


        }

        public bool CanBePlaced(GameObject newObject, Vector3 normal)
        {
            bool canBePlaced = false;

            normal = newObject.transform.InverseTransformVector(normal);
            Debug.Log(normal);

            BlockBase currentSelected = newObject.GetComponent<BlockBase>();
            if (currentSelected == null) return false;

            for (int i = 0; i < allowedDirections.Length; i++)
            {
                if (normal == allowedDirections[i])
                {
                    canBePlaced = true;
                    break;
                }
            }

            for (int i = 0; i < currentSelected.allowedDirections.Length; i++)
            {
                if (normal * -1 == currentSelected.allowedDirections[i])
                {
                    return canBePlaced;
                }
            }

            return false;
        }

        public void DeleteBlock()
        {
            
        }
    }
}
