using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(order = 0,fileName = "AI Data", menuName = "MetalSlug/AI")]
public class AIScriptableObject : ScriptableObject
{
    [Tooltip("Minimum distance to keep with the target")]
    [SerializeField] private float _minDistanceToKeepWithTarget = 3;
    [Tooltip("Range to allow Attack")]
    public float attackRange = 20;
    public float angleAim;
    public float speed = 1;
    public WeaponScriptableObject primaryWeapon;
    public WeaponScriptableObject grenadeWeapon;
    
    public float minDistanceToKeepWithTarget => _minDistanceToKeepWithTarget;

    [Header("Aerial Setting")] 
    public bool CanFly = false;
    public float minY = 3;
    

}
