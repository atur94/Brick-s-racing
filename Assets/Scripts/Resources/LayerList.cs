using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayerList : MonoBehaviour
{
    [SerializeField]
    public static LayerMask groundLayer;
    public LayerMask groundLayerSetter;



    public static LayerMask cubesLayer;
    public LayerMask cubesLayerSetter;

    public static LayerMask ignoreRaycastLayer;
    public LayerMask ignoreRaycastLayerSetter;

    public void Awake()
    {
        groundLayer = groundLayerSetter.value;
        cubesLayer = cubesLayerSetter.value;
        print(LayerMask.LayerToName(cubesLayer));

        ignoreRaycastLayer = ignoreRaycastLayerSetter.value;
        print(LayerMask.LayerToName(ignoreRaycastLayer));
    }

    public static int LayerMaskToLayerNumber(int layerMask)
    {
        return (int) (Mathf.Log((uint)layerMask, 2));
    }
}
