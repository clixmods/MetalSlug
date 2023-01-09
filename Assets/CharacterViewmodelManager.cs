using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewmodelManager : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject viewModel;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    

    public Vector3 Direction
    {
        set
        {
            var direction = value;
            var ogRotation = transform.eulerAngles;
            transform.LookAt(direction);
            transform.eulerAngles = new Vector3(ogRotation.x, transform.eulerAngles.y, ogRotation.z);

        }
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
