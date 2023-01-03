using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class AIInstance : MonoBehaviour
{
    const string Aidatadefault = "aiDataDefault";
    
    public AIScriptableObject aiScriptableObject;
    public GameObject target;
    private Rigidbody _rigidbody;
    private float _minDistanceToKeepWithTarget;
    private float _speed;

    public WeaponInstance currentWeapon;
    public WeaponInstance grenadeWeapon;
    
    //[Header("TEMPORAIRE A METTRE AU BON ENDROIT APRES")]
    //public GameObject prefabProjectile;
    #region Properties
    
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

        SpawnWeaponInstance();
        
        
        currentWeapon.transform.parent = transform;
        currentWeapon.transform.position = transform.position;
        grenadeWeapon.transform.parent = transform;
        grenadeWeapon.transform.position = transform.position;
        _minDistanceToKeepWithTarget =  Random.Range( aiScriptableObject.minDistanceToKeepWithTarget, aiScriptableObject.minDistanceToKeepWithTarget * 1.5f);
        _speed = Random.Range( aiScriptableObject.speed, aiScriptableObject.speed * 1.5f);
    }

    private void SpawnWeaponInstance()
    {
        currentWeapon = Instantiate(aiScriptableObject.primaryWeapon.prefabWeapon, transform.position, Quaternion.identity,
            transform).GetComponent<WeaponInstance>();
        grenadeWeapon = Instantiate(aiScriptableObject.grenadeWeapon.prefabWeapon, transform.position, Quaternion.identity,
            transform).GetComponent<WeaponInstance>();
    }

    // Start is called before the first frame update
    void Start()
    {
        target = LevelManager.GetRandomAlivePlayers.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            ThinkMovement();
            ThinkAttack();
            ThinkTargetPerception();
        }
        else
        {
            target = GetNearestPlayer();
        }
        
        
      
        
    }

    private void ThinkTargetPerception()
    {
        float distanceWithTarget = Vector3.Distance(transform.position, target.transform.position ) ;
        if (distanceWithTarget > aiScriptableObject.LoseSightRadius)
        {
            // TODO : Faire un getNearest ou utiliser radius perception
           // var newTarget = LevelManager.GetRandomAlivePlayers.gameObject;
            target = GetNearestPlayer();
        }
    }

    private void ThinkMovement()
    {
        float distanceWithTarget = Vector3.Distance(transform.position, target.transform.position);
//            Debug.Log(distanceWithTarget);
        if (distanceWithTarget > _minDistanceToKeepWithTarget)
        {
            Vector3 newPosition =
                Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * _speed);
            if (aiScriptableObject.CanFly)
            {
                newPosition.y = Mathf.Clamp(newPosition.y, aiScriptableObject.minY, 10);
            }

            _rigidbody.MovePosition(newPosition);
        }
    }

    private void ThinkAttack()
    {
        if (IsInAttackRange())
        {
            currentWeapon.DoFire(target);
        }
        else
        {
            grenadeWeapon.DoFire(target);
        }
    }

    private GameObject GetNearestPlayer()
    {
        var alivePlayers = LevelManager.GetAlivePlayers;
        GameObject nearestPlayer = null;
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            float distanceWithAlivePlayer = Vector3.Distance(transform.position, alivePlayers[i].transform.position);
            float distanceWithPotentialTarget = distanceWithAlivePlayer+1;
            if(nearestPlayer != null)
                 distanceWithPotentialTarget = Vector3.Distance(transform.position, nearestPlayer.transform.position);

            if (distanceWithAlivePlayer < distanceWithPotentialTarget)
            {
                nearestPlayer = alivePlayers[i].gameObject;
            }
        }

        return nearestPlayer;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (target != null)
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
            
        }
       
        
        
    }

    private bool IsInAttackRange()
    {
        var direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Abs(direction.y);
        return angle < aiScriptableObject.angleAim;
    }
}
