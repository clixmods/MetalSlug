using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BehaviorAfterPlay
{
    Nothing,
    DestroyAfterPlay
}
public class FXManager : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    private BehaviorAfterPlay _behaviorAfterPlay;
    public static FXManager InitFX(GameObject prefab,  Vector3 position, GameObject owner  = null)
    {
        Transform parent = null;
        FXManager fxManager = null;
      
        if (prefab != null)
        {
            var gameObject = Instantiate(prefab , position,  Quaternion.identity,parent);
            fxManager = gameObject.AddComponent<FXManager>();
        }
          
        if (owner != null)
        {
            parent = owner.transform;
            if ( fxManager != null && parent.TryGetComponent<CharacterViewmodelManager>(out var characterViewmodelManager))
                fxManager.skinnedMeshRenderer = characterViewmodelManager.skinnedMeshRenderer;

        }
        return fxManager;

        
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
     

    }

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        var shapeParameters = _particleSystem.shape;
        shapeParameters.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        shapeParameters.skinnedMeshRenderer = skinnedMeshRenderer;
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
