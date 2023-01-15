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

    #region Statics Methods
    public static FXManager InitFX(GameObject prefab,  Vector3 position, GameObject owner  = null, SkinnedMeshRenderer skinnedMeshRenderer = null)
    {
        Transform parent = null;
        FXManager fxManager = null;
      
        if (prefab != null)
        {
            var gameObject = Instantiate(prefab , position,  prefab.transform.rotation,parent);
            fxManager = gameObject.AddComponent<FXManager>();
        }
          
        if (owner != null)
        {
            parent = owner.transform; 
            if (fxManager != null)
            {
                if (parent.TryGetComponent<CharacterViewmodelManager>(out var characterViewmodelManager))
                {
                    if (skinnedMeshRenderer != null)
                    {
                        fxManager.skinnedMeshRenderer = skinnedMeshRenderer;
                    }
                }
                fxManager.transform.parent = owner.transform;
            }
        }
        return fxManager;
    }
    public static FXManager PlayFX(FXManager fxManager, Vector3 position = default, BehaviorAfterPlay behaviorAfterPlay = BehaviorAfterPlay.Nothing)
    {
        if (fxManager == null)
        {
            return null;
        }
        return fxManager.Play(position,behaviorAfterPlay);
    }

    #endregion
    
    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        var shapeParameters = _particleSystem.shape;
        if(shapeParameters.shapeType == ParticleSystemShapeType.SkinnedMeshRenderer)
        {
            //shapeParameters.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
            shapeParameters.skinnedMeshRenderer = skinnedMeshRenderer;
        }
    }

    private FXManager Play(Vector3 position, BehaviorAfterPlay behaviorAfterPlay = BehaviorAfterPlay.Nothing)
    {
        gameObject.SetActive(true);
        if (_particleSystem == null)
        {
            Debug.Log("FX Null", gameObject);
            return null;
        }
        _behaviorAfterPlay = behaviorAfterPlay;
        if(position != default)
            _particleSystem.transform.position = position;
        
        _particleSystem.Play();
        return this;
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
