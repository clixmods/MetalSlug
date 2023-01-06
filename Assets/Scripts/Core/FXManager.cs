using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviorAfterPlay
{
    Nothing,
    DestroyAfterPlay
}
public class FXManager : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private BehaviorAfterPlay _behaviorAfterPlay;
    public static FXManager InitFX(GameObject prefab,  Vector3 position, GameObject owner  = null)
    {
        Transform parent = null;
        if (owner != null)
        {
            parent = owner.transform;
        }

        if (prefab != null)
        {
            var gameObject = Instantiate(prefab , position,  Quaternion.identity,parent);
            var fxManager = gameObject.AddComponent<FXManager>();
         
            return fxManager;
        }

        return null;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play(Vector3 position, BehaviorAfterPlay behaviorAfterPlay = BehaviorAfterPlay.Nothing)
    {
        _behaviorAfterPlay = behaviorAfterPlay;
        _particleSystem.transform.position = position;
        _particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (_behaviorAfterPlay == BehaviorAfterPlay.DestroyAfterPlay && !_particleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
