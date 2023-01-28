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

    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    public SkinnedMeshRenderer SkinnedMeshRenderer => _skinnedMeshRenderer;
    private Animator _animator;
    public bool ManageIsFalling = true;
    public static readonly int IsFalling = Animator.StringToHash("IsFalling");
    public static readonly int ShootingUp = Animator.StringToHash("ShootingUp");
    public static readonly int Shooting = Animator.StringToHash("Shooting");
    public static readonly int IsRunning = Animator.StringToHash("IsRunning");
    public static readonly int ShootingDown = Animator.StringToHash("ShootingDown");
    public static readonly int LookUp = Animator.StringToHash("LookUp");
    public static readonly int LookDown = Animator.StringToHash("LookDown");
    public static readonly int Down = Animator.StringToHash("Down");
    public static readonly int Revived = Animator.StringToHash("Revived");

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
        _skinnedMeshRenderer ??= GetComponentInChildren<SkinnedMeshRenderer>();
        _animator = GetComponentInChildren<Animator>();
        leftHand ??= gameObject;
        rightHand ??= gameObject;
    }

    public bool GetAnimatorBool(int id)
    {
        return _animator.GetBool(id);
    }

    public void SetAnimatorBool(int id, bool value)
    {
        _animator.SetBool(id, value);
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
                _animator.SetBool(IsRunning, false);
                break;
            case AnimState.Move:
                _animator.SetBool(IsRunning, true);
                break;
            case AnimState.Fire:
                _animator.SetTrigger(Shooting);
                break;
            case AnimState.FireUp:
                _animator.SetTrigger(ShootingUp);
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
                _animator.SetTrigger(ShootingDown);
                break;
            case AnimState.LookUp:
                _animator.SetTrigger(LookUp);
                break;
            case AnimState.LookDown:
                _animator.SetTrigger(LookDown);
                break;
            case AnimState.Down:
                _animator.SetTrigger(Down);
                break;
            case AnimState.Revived:
                _animator.SetTrigger(Revived);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (ManageIsFalling)
        {
            if (_rigidbody.velocity.y is > 0.1f or < -0.1f)
            {
                _animator.SetBool(IsFalling, true);
            }
            else
            {
                _animator.SetBool(IsFalling, false);
            }

            if (_rigidbody.velocity.magnitude > 0)
            {
                //
            }
        }
       
    }
}
