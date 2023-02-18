using System;
using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayLoopAlias : MonoBehaviour
{
    [Aliase] [SerializeField] private string aliasToPlay;
    private AudioPlayer _audioPlayer;
    private void Start()
    {
        PlayAlias();
    }

    public void PlayAlias()
    {
        transform.PlayLoopSound(aliasToPlay,ref _audioPlayer );
    }
}
