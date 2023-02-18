using System;
using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayAliasListener : MonoBehaviour
{
    [Aliase] [SerializeField] private string aliasToPlay;

    private void Start()
    {
        PlayAlias();
    }

    public void PlayAlias()
    {
        transform.PlaySoundAtPosition(aliasToPlay);
    }
}