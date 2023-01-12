using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var VARIABLE in meshRenderers)
        {
            Destroy(VARIABLE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
