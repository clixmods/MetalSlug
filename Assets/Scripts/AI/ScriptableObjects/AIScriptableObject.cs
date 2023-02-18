using System.Collections;
using System.Collections.Generic;
using AudioAliase;
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

    public float attackRate = 0.5f;
    public float angleAim;
    public bool canReajustAim = true;
    [SerializeField] private float speed = 1;
    public float Speed => Random.Range( speed, speed * 1.5f);
    public WeaponScriptableObject primaryWeapon;
    public WeaponScriptableObject grenadeWeapon;
    
    public float minDistanceToKeepWithTarget => _minDistanceToKeepWithTarget;

    [Header("Aerial Setting")] 
    public bool CanFly = false;
    public bool LeftRightPattern;
    public float minY = 3;

    [Header("Detection")] 
    public bool CanLoseTarget;
    [Tooltip("The max distance over which this sense can start perceiving.")]
    public float SightRadius = 5;
    [Tooltip("The max distance in which a seen target is no longer perceived by the sight sense.")]
    public float LoseSightRadius = 10;

    [Header("Aliases")] 
    [Aliase] public string AliasOnDeath;
    [Aliase] public string AliasOnMove;
    [Aliase] public string AliasOnSpawn;
    [Aliase] public string AliasOnAmbiant;
    [Aliase] public string AliasOnHit;
    [Aliase] public string AliasOnDamaged;
    [Aliase] public string AliasOnDamagedLoop;
    [Aliase] public string AliasOnLowHealth;
    [Aliase] public string AliasOnLowHealthLoop;


    [Header("Score")]
    public int ScoreDead = 50;
    public int ScoreHit = 10;
    public int IncreaseMultiplier = 1;
    public bool IncreaseMultiplierOnHit = false;
    [Header("FX")] 
    public GameObject FXLoopAmbiant;
    public GameObject FXDeath;
    public GameObject FXHit;
    public GameObject FXLoopLowHealth;
    public GameObject FXLoopDamaged;
    public GameObject FXMove;

    [Header("DIVERS")] public bool EarthquakeOnDeath;
}
