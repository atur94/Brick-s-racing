using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformControl : MonoBehaviour
{
    private GameObject platform;

    [SerializeField]
    private float angularVelocity = 0.5f;

    [SerializeField]
    private Grid grid;

    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, 0.5f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, -0.5f);

        }
    }
}
