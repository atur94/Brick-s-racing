using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public abstract class BlockBase : MonoBehaviour, IBlockType
    {
        [SerializeField]
        protected GameObject[] connectedBlocks;

        public Type[] Compatibility;
        public Type[] ObjectsAllowedToSnap =
        {
            typeof(SnappableComponent),
            typeof(BlockBase),
        };

        public Type[] ObjectsNotAllowedToSnap;

        public GameObject[] Attached;

        public bool HasNeighbour = false;
        protected virtual bool IsNewCompatible(BlockBase parent)
        {
            if (parent == null) return false;
            bool isCompatible = false;
            var type = GetType();

            if (parent.ObjectsAllowedToSnap != null)
            {
                for (int i = 0; i < parent.ObjectsAllowedToSnap.Length; i++)
                {
                    if (type == parent.ObjectsAllowedToSnap[i] || type.IsSubclassOf(parent.ObjectsAllowedToSnap[i]))
                    {
                        isCompatible = true;
                        break;
                    }

                }
            }

            if (parent.ObjectsNotAllowedToSnap != null)
            {
                for (int i = 0; i < parent.ObjectsNotAllowedToSnap.Length; i++)
                {
                    if (type == parent.ObjectsNotAllowedToSnap[i] || type.IsSubclassOf(parent.ObjectsNotAllowedToSnap[i]))
                    {
                        isCompatible = false;
                        break;
                    }

                }
            }

            return isCompatible;
        }

        public Vector3[] allowedPlaceDirections =
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

        private bool isKinematic = true;

        private void Awake()
        {
            Init();

            connectedBlocks = new GameObject[allowedPlaceDirections.Length];
        }

        protected abstract void Init();

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if(rb == null) print("Rigidbody for this block is null");
            rb.isKinematic = isKinematic;
            rotatedDirections = new Vector3[allowedPlaceDirections.Length];
            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                rotatedDirections[i] = transform.rotation * allowedPlaceDirections[i];
            }
        }

        public new void ToString()
        {
            Debug.Log($"Jestem {this.GetType()}");
        }

        public Vector3[] AllowedDirections
        {
            get => allowedPlaceDirections;
            set => allowedPlaceDirections = value;
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

        private bool rotated = false;
        public void Rotate(Vector3 axis, float angle, Space relativeTo)
        {
            transform.Rotate(axis, angle, relativeTo);
            rotatedDirections = new Vector3[allowedPlaceDirections.Length];
            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                rotatedDirections[i] = transform.rotation * allowedPlaceDirections[i];
            }
            TestForNeighboursChild(gameObject, false);
            TestForNeighbours();
            TestForNeighboursChild(gameObject, false);
        }

        public void DrawRays()
        {
            foreach (var allowedDirection in rotatedDirections)
            {
                Debug.DrawRay(transform.position, allowedDirection*3, Color.yellow, 0.01f);
            }
        }

        public void refreshRotatedVectors()
        {
            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                rotatedDirections[i] = transform.rotation * allowedPlaceDirections[i];
            }
        }

        public void TestForNeighbours()
        {
            bool addedElement = false;
            connectedBlocks = new GameObject[allowedPlaceDirections.Length];
            int elements = 0;
            rotatedDirections = new Vector3[allowedPlaceDirections.Length];
            RaycastHit info;
            Ray ray;

            for (var index = 0; index < rotatedDirections.Length; index++)
            {
                rotatedDirections[index] = transform.rotation * allowedPlaceDirections[index];

                Debug.DrawRay(transform.position, rotatedDirections[index]*0.55f, Color.magenta, 3f);
                ray = new Ray(transform.position, rotatedDirections[index]);
                if (Physics.Raycast(ray, out info, 1f, LayerList.cubesLayer | LayerList.groundLayer))
                {
                    var collidingObject = info.collider.gameObject.GetComponent<BlockBase>();
                    if (collidingObject != null)
                    {
                        Debug.Log(collidingObject.name);
                        collidingObject.refreshRotatedVectors();
                        for (int i = 0; i < collidingObject.allowedPlaceDirections.Length; i++)
                        {
                            if (collidingObject.rotatedDirections[i] * -1 == rotatedDirections[index])
                            {
                                connectedBlocks[index] = info.collider.gameObject;
                                addedElement = true;
                                elements++;
                                break;
                            }
                        }
                    }

                }
            }

            HasNeighbour = addedElement;
        }

        void TestForNeighboursChild(GameObject parent, bool triggerEvent = true)
        {
            rotatedDirections = new Vector3[allowedPlaceDirections.Length];
            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                rotatedDirections[i] = transform.rotation * allowedPlaceDirections[i];
            }
            for (var i = 0; i < connectedBlocks.Length; i++)
            {

                var connectedGameObject = connectedBlocks[i];
                if (connectedGameObject == null) continue;
                BlockBase block = connectedGameObject.GetComponent<BlockBase>();
                if (block != null)
                {
                    block.TestForNeighbours();
                }

            }
            if (triggerEvent)
                OnBlockPlaced(parent);
            rotated = false;
        }

        IEnumerator NeighbourTestCoroutine(GameObject parent)
        {
            yield return new WaitForFixedUpdate();
            TestForNeighboursChild(parent);
        }

        protected virtual void OnBlockPlaced(GameObject parent)
        {

        }

        public void BlockPlaced(GameObject parent)
        {
            rotatedDirections = new Vector3[allowedPlaceDirections.Length];
            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                rotatedDirections[i] = transform.localRotation * allowedPlaceDirections[i];
            }
            TestForNeighbours();
            StartCoroutine(NeighbourTestCoroutine(parent));
//            OnBlockPlaced(parent);

        }

        public bool CanBePlacedChild(GameObject parent, Vector3 normal)
        {
            if (parent == null) return false;
            ObjectVectors normals;
            normals.parentHitNormal = parent.transform.InverseTransformVector(normal);
            normals.currentObjectNormal = transform.InverseTransformVector(normal);
            normals.Normal = normal;

            return CanBePlacedChild(parent, ref normals);
        }

        public virtual bool CanBePlacedChild(GameObject parent, ref ObjectVectors vectors)
        {
            bool canBePlacedChild = false;
            bool canBePlacedParent = false;

//            Debug.Log($"parentHitNormal = {vectors.parentHitNormal}, normal = {vectors.currentObjectNormal}");

            BlockBase parentBlock = parent.GetComponent<BlockBase>();
            if (parentBlock == null) return false;
            if (!IsNewCompatible(parentBlock)) return false;
            Vector3[] parentDirections = parentBlock.allowedPlaceDirections;
            if (parentBlock is SnappableComponent snappableParent)
            {
                parentDirections = snappableParent.SnapDirections;
            }

            for (int i = 0; i < parentDirections.Length; i++)
            {
                if (vectors.parentHitNormal == parentDirections[i])
                {
                    canBePlacedParent = true;
                    break;
                }
            }

            if (!canBePlacedParent) return false;

            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                if (vectors.currentObjectNormal * -1 == allowedPlaceDirections[i])
                {
                    canBePlacedChild = true;
                    break;
                }
            }

            return canBePlacedParent && canBePlacedChild;
        }

        public bool CanBePlacedParent(GameObject newObject, Vector3 normal)
        {
            if (newObject == null) return false;
            ObjectVectors normals;
            normals.parentHitNormal = transform.InverseTransformVector(normal);
            normals.currentObjectNormal = newObject.transform.InverseTransformVector(normal);
            normals.Normal = normal;
            return CanBePlacedParent(newObject, ref normals);
        }

        protected virtual bool CanBePlacedParent(GameObject newObject, ref ObjectVectors vectors)
        {
            bool canBePlacedChild = false;
            bool canBePlacedParent = false;
            if (newObject == null) return false;

            Debug.Log($"parentHitNormal = {vectors.parentHitNormal}, normal = {vectors.currentObjectNormal}");
            BlockBase currentSelected;
            currentSelected = newObject.GetComponent<BlockBase>();

            if (currentSelected == null) return false;
            if (!currentSelected.IsNewCompatible(this)) return false;

            for (int i = 0; i < allowedPlaceDirections.Length; i++)
            {
                if (vectors.parentHitNormal == allowedPlaceDirections[i])
                {
                    canBePlacedParent = true;
                    break;
                }
            }

            if (!canBePlacedParent) return false;

            for (int i = 0; i < currentSelected.allowedPlaceDirections.Length; i++)
            {
                if (vectors.currentObjectNormal * -1 == currentSelected.allowedPlaceDirections[i])
                {
                    canBePlacedChild = true;
                    break;
                }
            }

            return canBePlacedParent && canBePlacedChild;
        }

        public void DeleteBlock()
        {
            OnDeleteBlock();
            Destroy(gameObject);
        }

        protected virtual void OnDeleteBlock() { }
        public virtual void AddAttached(Attachable attachable)
        {
            if (attachable == null && Attached == null) return;
            for (int i = 0; i < Attached.Length; i++)
            {
                if (Attached[i] == null)
                {
                    Attached[i] = attachable.gameObject;
                    return;
                }
            }
        }
    }
    public struct ObjectVectors
    {
        public Vector3 Normal;
        public Vector3 parentHitNormal;
        public Vector3 currentObjectNormal;
    }
}
