using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(order = 0,fileName = "AI Data", menuName = "MetalSlug/AI")]
public class AIScriptableObject : ScriptableObject
{
    [Tooltip("Minimum distance to keep with the target")]
    public TeamEnum team;
    [SerializeField] private float _minDistanceToKeepWithTarget = 3;
    public int Health = 1;
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

    [Header("Detection")] 
    public bool CanLoseTarget;
    [Tooltip("The max distance over which this sense can start perceiving.")]
    public float SightRadius = 5;
    [Tooltip("The max distance in which a seen target is no longer perceived by the sight sense.")]
    public float LoseSightRadius = 10;

    
}
