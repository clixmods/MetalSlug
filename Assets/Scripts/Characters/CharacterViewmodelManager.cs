using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AnimState
{
    Idle, 
    Move,
    Fire,
    FireUp,
    FireDown,
    LookUp,
    LookDown,
    Grenade,
    Damaged,
    Down,
    Revived,
    Falling
    
}
public class CharacterViewmodelManager : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject viewModel;
    public GameObject leftHand;
    public GameObject rightHand;


    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Animator _animator;

    private const int LOWERBODY = 1;
    private const int UPPERBODY = 2;
    
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
        if (viewModel == null)
        {
            Debug.LogWarning("viewModel is not assigned", gameObject);
            this.enabled = false;
        }
        _rigidbody = GetComponent<Rigidbody>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _animator = GetComponentInChildren<Animator>();
        leftHand ??= gameObject;
        rightHand ??= gameObject;

    }

    public void Play(AnimState state)
    {
        if (_animator == null)
        {
            Debug.LogWarning("No Animator for this actor ", gameObject);
            return;
            
        }
        switch (state)
        {
            case AnimState.Idle:
                //_animator.Play("Idle", LOWERBODY );
                _animator.SetBool("IsRunning", false);
                break;
            case AnimState.Move:
                _animator.SetBool("IsRunning", true);
                // if(!_animator.GetCurrentAnimatorStateInfo(LOWERBODY).IsName("Running"))
                //     _animator.Play("Running", LOWERBODY );
                // if(!_animator.GetCurrentAnimatorStateInfo(UPPERBODY).IsName("Running"))
                //     _animator.Play("Running", UPPERBODY );
                break;
            case AnimState.Fire:
                _animator.SetTrigger("Shooting");
                //_animator.Play("Shooting", UPPERBODY );
                break;
            case AnimState.FireUp:
                _animator.SetTrigger("ShootingUp");
                //_animator.Play("Shooting", UPPERBODY );
                break;
            case AnimState.Damaged:
               // _animator.Play("Fall", UPPERBODY );
                break;
            case AnimState.Grenade:
                //_animator.Play("Grenade", UPPERBODY );
                break;
            case AnimState.Falling:
                //_animator.SetBool("IsFalling", true);
                break;
            case AnimState.FireDown:
                _animator.SetTrigger("ShootingDown");
                break;
            case AnimState.LookUp:
                _animator.SetTrigger("LookUp");
                break;
            case AnimState.LookDown:
                _animator.SetTrigger("LookDown");
                break;
            case AnimState.Down:
                _animator.SetTrigger("Down");
                break;
            case AnimState.Revived:
                _animator.SetTrigger("Revived");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_rigidbody.velocity.y > 0.1f || _rigidbody.velocity.y < -0.1f)
        {
            _animator.SetBool("IsFalling", true);
        }
        else
        {
            _animator.SetBool("IsFalling", false);
        }

        if (_rigidbody.velocity.magnitude > 0)
        {
          //
        }
    }
}
