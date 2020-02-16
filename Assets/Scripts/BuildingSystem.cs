using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Resources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera playerCamera;

    private bool buildModeOn = false;
    private bool canBuild = false;
    private bool isBlockAllowed = false;
    private bool destroyModeOn = false;
    private bool canDestroy = false;
    private bool editModeOn = true;
    private bool canEdit = false;


    private BlockSystem bSys;

    private Vector3 buildPos;
    private Vector3 destroyPos;

    private Quaternion buildQuaternion;
    private Quaternion destroyQuaternion;

    private GameObject currentTemplateBlock;
    private GameObject destroyTemplateBlock;
    private GameObject blockToDestroy;
    [FormerlySerializedAs("elementCurrentlySelectedPrefab")] [SerializeField]
    private GameObject selectedObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject blockTemplatePrefab;

    [SerializeField]
    private GameObject destroyTemplatePrefab;

    [SerializeField]
    private GameObject blockPrefab;

    [Header("Materials")]

    [SerializeField]
    private Material templateMaterial;

    [SerializeField]
    private Material selectedMaterial;

    [SerializeField] 
    private Material isNotAllowedMaterial;

    [Header("Helpers")]

    [SerializeField]
    private GameObject plane;

    [SerializeField]
    private int selectedBlockId = 0;

    private Material temporaryMaterial;
    

    void Start()
    {
        bSys = GetComponent<BlockSystem>();
    }

    private Dictionary<BlockBase, int> test;

    private GameObject parentBlock;
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            destroyModeOn = false;
            buildModeOn = !buildModeOn;
        }

        if (Input.GetKeyDown("r"))
        {
            buildModeOn = false;
            destroyModeOn = !destroyModeOn;
        }

        if (!destroyModeOn && !buildModeOn)
        {
            editModeOn = true;
        }
        else
        {
            editModeOn = false;
        }

        if (buildModeOn)
        {
            RaycastHit buildPosHit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            BlockTypeSelector();
            if (Physics.Raycast(ray, out buildPosHit, Mathf.Infinity, LayerList.cubesLayer))
            {
                parentBlock = buildPosHit.collider.gameObject;
                Vector3 point = buildPosHit.collider.transform.localPosition;
                buildQuaternion = buildPosHit.transform.rotation;
                BlockBase currentBlock = null;

                if (currentTemplateBlock != null)
                {
                    BoxCollider currentBoxCollider = currentTemplateBlock.GetComponent<BoxCollider>();
                    currentBlock = currentTemplateBlock.gameObject.GetComponent<BlockBase>();
                    if (currentBoxCollider != null)
                    {
                        var v = plane.transform.InverseTransformVector(buildPosHit.normal);
                        
                        var normalColliderRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z)));
                        var t = normalColliderRotation * (currentBoxCollider.size + currentBoxCollider.center);
                        Debug.Log($"1 = {Vector3.Scale(t,v)} 2 = {v}");

                        point += 0.5f*v + 0.5f * Vector3.Scale(normalColliderRotation *
                                                            (currentBoxCollider.size + currentBoxCollider.center), v);
                    }
                    else
                    {
                        point += plane.transform.InverseTransformVector(buildPosHit.normal);
                    }
                    //                    point += Vector3.Scale(plane.transform.InverseTransformVector(buildPosHit.normal)*0.5f, currentTemplateBlock.GetComponent<Collider>().bounds.size * 0.5f);
                    //                    point += 0.5f * plane.transform.InverseTransformVector(buildPosHit.normal);


                }
                else
                {
                }

                if (currentBlock != null && currentBlock.CanBePlacedChild(buildPosHit.collider.gameObject, buildPosHit.normal))
                {
                    isBlockAllowed = true;

                }
                else
                {
                    isBlockAllowed = false;

                }
                buildPos = point;
                canBuild = true;
            }
            else if (Physics.Raycast(ray, out buildPosHit, Mathf.Infinity, LayerList.groundLayer) )
            {
                var local = plane.transform.InverseTransformPoint(buildPosHit.point);
                Vector3 point = new Vector3(Mathf.Round(local.x), Mathf.Round(local.y), Mathf.Round(local.z)) + buildPosHit.normal * 1f;
                buildQuaternion = buildPosHit.transform.rotation;

                buildPos = point;
                canBuild = true;
                isBlockAllowed = true;
                parentBlock = buildPosHit.collider.gameObject;
            }
            else
            {
                if(currentTemplateBlock != null)
                    Destroy(currentTemplateBlock);
                canBuild = false;
                isBlockAllowed = false;
                parentBlock = null;
            }
        }

        if (!buildModeOn && currentTemplateBlock != null)
        {
            Destroy(currentTemplateBlock.gameObject);
            canBuild = false;
        }

        if (canBuild && currentTemplateBlock == null && blockTemplatePrefab != null)
        {
            currentTemplateBlock = Instantiate(blockTemplatePrefab, plane.transform);
            var rBody = currentTemplateBlock.GetComponent<Rigidbody>();
            if (rBody != null)
            {
//                rBody.isKinematic = true;
//                rBody.detectCollisions = false;
            }
                
            currentTemplateBlock.layer = LayerList.LayerMaskToLayerNumber(LayerList.ignoreRaycastLayer);
            currentTemplateBlock.transform.localPosition = buildPos;
            currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial;
        }



        if (canBuild && currentTemplateBlock != null)
        {
            currentTemplateBlock.transform.localPosition = buildPos;
            ChangeGameObjectRotation(currentTemplateBlock.GetComponent<BlockBase>());
//            currentTemplateBlock.transform.rotation = grid.transform.rotation;

//            Debug.DrawLine(plane.transform.position + buildPos, plane.transform.position + buildPos + Vector3.up*2, Color.magenta, 0.01f);
            
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Debug.DrawLine(buildPos, new Vector3(buildPos.x + j, buildPos.y, buildPos.z + i), Color.cyan, 0.1f);
                }
            }

            if (isBlockAllowed == false)
            { 
                currentTemplateBlock.GetComponent<MeshRenderer>().material = isNotAllowedMaterial;
            }
            else
            {
                if(blockTemplatePrefab != null)
                    currentTemplateBlock.GetComponent<MeshRenderer>().material =
                        blockTemplatePrefab.GetComponent<MeshRenderer>().sharedMaterial;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if(isBlockAllowed)
                    PlaceBlock();
            }
        }
        DestroySystem();
        SelectSystem();


    }

    private void PlaceBlock()
    {
        Block tempBlock = bSys.allBlocks[selectedBlockId];
        GameObject newBlock = Instantiate(tempBlock.prefab, plane.transform);
        newBlock.layer = LayerList.LayerMaskToLayerNumber(LayerList.cubesLayer);
        newBlock.transform.localPosition = buildPos;
        newBlock.transform.rotation = currentTemplateBlock.transform.rotation;
        newBlock.name = tempBlock.blockName + "-block-"+ (new System.Random().Next(0, 1000));
        newBlock.GetComponent<BlockBase>()?.BlockPlaced(parentBlock);
        if(tempBlock.overrideMaterial)
            newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
    }

    private void BlockTypeSelector()
    {
        if(buildModeOn)
        {
            int blockDictionaryElements = bSys.allBlocks.Count;
            Dictionary<int, Block> allBlocks = bSys.allBlocks;

            if(blockTemplatePrefab == null)
            {
                blockTemplatePrefab = allBlocks[0].prefab;
            }

            for (int i = 1; i <= blockDictionaryElements; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    selectedBlockId = i - 1;
                    blockTemplatePrefab = allBlocks[i-1].prefab;
                    if (currentTemplateBlock != null)
                    {
                        Destroy(currentTemplateBlock);
                    }
                }
            }
        }
    }

    private void DeleteBlock(GameObject blockToDestroy)
    {
        if(blockToDestroy != null)
        {
            var block = blockToDestroy.GetComponent<BlockBase>();
            if(block != null) block.DeleteBlock();
            else
                Destroy(blockToDestroy);
        }
    }

    private void DestroySystem()
    {
        if (destroyModeOn)
        {
            RaycastHit destroyPosHit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,
                out destroyPosHit, Mathf.Infinity, LayerList.cubesLayer))
            {

                Vector3 point = destroyPosHit.collider.transform.localPosition;
                blockToDestroy = destroyPosHit.collider.gameObject;
                destroyPos = point;
                canDestroy = true;
                
            }
            else
            {
                canDestroy = false;
                if (destroyTemplateBlock != null)
                {
                    Destroy(destroyTemplateBlock);
                }

                blockToDestroy = null;

            }
        }

        if (!destroyModeOn && destroyTemplateBlock != null)
        {
            Destroy(destroyTemplateBlock);
            blockToDestroy = null;
            canDestroy = false;
        }

        if (canDestroy && destroyTemplateBlock == null)
        {
            destroyTemplateBlock = Instantiate(destroyTemplatePrefab, plane.transform);
            destroyTemplateBlock.transform.localPosition = destroyPos;
        }


        if (canDestroy && destroyTemplateBlock != null)
        {
            destroyTemplateBlock.transform.localPosition = destroyPos;
            destroyTemplateBlock.transform.rotation = plane.transform.rotation;

        }

        if (Input.GetMouseButtonDown(0))
        {
            DeleteBlock(blockToDestroy);
        }
    }

    private void SelectSystem()
    {
        if (editModeOn)
        {
            RaycastHit selectPosHit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out selectPosHit, Mathf.Infinity, LayerList.cubesLayer))
                {

                    if (selectedObject != null && temporaryMaterial != null)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = temporaryMaterial;
                        temporaryMaterial = null;
                        selectedObject = null;
                    }

                    selectedObject = selectPosHit.collider.gameObject;
                    selectedObject.GetComponent<BlockBase>().ToString();
                    temporaryMaterial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = selectedMaterial;
                    
                }
                else
                {
                    if (selectedObject != null )
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = temporaryMaterial;
                        temporaryMaterial = null;
                        selectedObject = null;
                    }
                }

            }
        }

        

        if (!editModeOn && selectedObject != null )
        {
            selectedObject.GetComponent<MeshRenderer>().material = temporaryMaterial;
            selectedObject = null;
            temporaryMaterial = null;
        }

        if (editModeOn && selectedObject != null)
        {
            if (Input.GetKeyDown("x"))
            {
                DeleteBlock(selectedObject);
                selectedObject = null;
                temporaryMaterial = null;
            }


            if(selectedObject != null)
            {
                ChangeGameObjectRotation(selectedObject.GetComponent<BlockBase>());
            }
        }
    }

    void ChangeGameObjectRotation(IBlockType obj)
    {
        if (obj == null) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            obj.Rotate(Vector3.up, -90f, Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            obj.Rotate(Vector3.up, 90f, Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            obj.Rotate(Vector3.right, 180f, Space.Self);
        }
    }
}
