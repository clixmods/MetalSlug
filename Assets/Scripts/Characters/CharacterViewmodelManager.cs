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
    Dictionary<int, bool> animNames = new Dictionary<int, bool>();
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
        // Cache available parameters in animator
        foreach (AnimatorControllerParameter param in _animator.parameters) 
        {
            animNames.Add(param.nameHash,true);
        }
        leftHand ??= gameObject;
        rightHand ??= gameObject;
    }

    public bool GetAnimatorBool(int id)
    {
        if(animNames.ContainsKey(id))
            return _animator.GetBool(id);
        
        return false;
    }
    public void SetAnimatorBool(int id, bool value)
    {
        if(animNames.ContainsKey(id))
            _animator.SetBool(id, value);
    }

    private void SetAnimatorTrigger(int id)
    {
        if (animNames.ContainsKey(id))
        {
            _animator.SetTrigger(id);
        }
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
                SetAnimatorBool(IsRunning, false);
                break;
            case AnimState.Move:
                SetAnimatorBool(IsRunning, true);
                break;
            case AnimState.Fire:
                SetAnimatorTrigger(Shooting);
                break;
            case AnimState.FireUp:
                SetAnimatorTrigger(ShootingUp);
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
                SetAnimatorTrigger(ShootingDown);
                break;
            case AnimState.LookUp:
                SetAnimatorTrigger(LookUp);
                break;
            case AnimState.LookDown:
                SetAnimatorTrigger(LookDown);
                break;
            case AnimState.Down:
                SetAnimatorTrigger(Down);
                break;
            case AnimState.Revived:
                SetAnimatorTrigger(Revived);
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
                SetAnimatorBool(IsFalling, true);
            }
            else
            {
                SetAnimatorBool(IsFalling, false);
            }

            if (_rigidbody.velocity.magnitude > 0)
            {
                //
            }
        }
       
    }
}
