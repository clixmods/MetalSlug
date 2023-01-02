using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(order = 0,fileName = "AI Data", menuName = "MetalSlug/AI")]
public class AIScriptableObject : ScriptableObject
{
    [Tooltip("Minimum distance to keep with the target")]
    public float minDistanceToKeepWithTarget = 3;
    [Tooltip("Range to allow Attack")]
    public float attackRange = 20;

    public float angleAim;
}
