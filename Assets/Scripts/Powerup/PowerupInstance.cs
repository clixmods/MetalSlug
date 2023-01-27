using System;
using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PowerupInstance : MonoBehaviour, IPowerupInstance
{
    #region Events
    public delegate void PowerUpEvent(PowerupInstance powerupInstance);
    public static PowerUpEvent onPowerupPickup;
    #endregion
    
    private bool _isGrabbed;
    [Header("Aliases")]
    [SerializeField][Aliase] private string aliasOnSpawn;
    [SerializeField][Aliase] private string aliasOnIdleLoop;
    [SerializeField][Aliase] private string aliasOnGrab;
    private AudioPlayer _audioPlayerLoopIdle;


    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 50f);
    }

    public virtual void OnEnable()
    {
        OnSpawn();
        transform.PlayLoopSound(aliasOnIdleLoop, ref _audioPlayerLoopIdle);
        transform.PlaySoundAtPosition(aliasOnSpawn);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (_isGrabbed)
        {
            return;
        }
        if ( other.transform.parent != null && other.transform.parent.CompareTag("Player") )
        {
            var gameObjectPlayer = other.transform.parent;
            if (gameObjectPlayer.TryGetComponent<PlayerInstance>(out var playerInstance))
            {
                _isGrabbed = true;
                OnGrab(playerInstance);
            }
        }
    }

    public virtual void OnSpawn()
    {
        _isGrabbed = false;
    }

    public virtual void OnGrab(PlayerInstance playerInstance)
    {
        transform.PlaySoundAtPosition(aliasOnSpawn);
        AudioManager.StopLoopSound(ref _audioPlayerLoopIdle, StopLoopBehavior.FinishCurrentPlay);
        onPowerupPickup?.Invoke(this);
        Destroy(gameObject);
    }
}
