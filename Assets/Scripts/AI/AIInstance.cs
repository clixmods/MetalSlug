using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIInstance : MonoBehaviour
{
    const string Aidatadefault = "aiDataDefault";
    
    public AIScriptableObject aiScriptableObject;
    public GameObject target;
    private Rigidbody _rigidbody;

    public WeaponInstance currentWeapon;

    //[Header("TEMPORAIRE A METTRE AU BON ENDROIT APRES")]
    //public GameObject prefabProjectile;
    #region Properties
    private float MinDistanceToKeepWithTarget => aiScriptableObject.minDistanceToKeepWithTarget;
    private float AttackRange => aiScriptableObject.minDistanceToKeepWithTarget;

    #endregion

    private void OnValidate()
    {
        if (aiScriptableObject == null)
        {
            aiScriptableObject = (AIScriptableObject)Resources.Load(Aidatadefault);
            Debug.LogWarning("AI Instance doesn't have aiScriptableObject variable assigned, please assign it", gameObject);
        }
    }

    private void Awake()
    {
        // Prevent if aiScriptableObject is null, and set a default scriptableObject
        if (aiScriptableObject == null)
        {
            aiScriptableObject = (AIScriptableObject)Resources.Load(Aidatadefault);
            Debug.LogWarning("AI Instance doesn't have aiScriptableObject variable assigned, please assign it", gameObject);
        }
        _rigidbody = GetComponent<Rigidbody>();
        currentWeapon.transform.parent = transform;
        currentWeapon.transform.position = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            float distanceWithTarget = Vector3.Distance(transform.position, target.transform.position);
            Debug.Log(distanceWithTarget);
            if ( distanceWithTarget > MinDistanceToKeepWithTarget)
            {
                _rigidbody.MovePosition(Vector3.Lerp(transform.position ,target.transform.position, Time.deltaTime ));
            }
        }

        if (IsInAttackRange())
        {
            currentWeapon.DoFire(target);

        }
        else
        {
            currentWeapon.DoGrenade(target);
        }
        
    }
    

    private void OnDrawGizmos()
    {
        var direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Abs(direction.y);
        Handles.Label(transform.position, 
            $"Angle {angle }");
        if (IsInAttackRange())
        {
           
            Debug.DrawLine(transform.position, target.transform.position ,Color.green);
        }
        else
        {
        
            Debug.DrawLine(transform.position, target.transform.position,Color.red);
            
        }
        
        
    }

    private bool IsInAttackRange()
    {
        var direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Abs(direction.y);
        return angle < aiScriptableObject.angleAim;
    }
}
