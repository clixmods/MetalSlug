using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayAliasListener : MonoBehaviour
{
    [Aliase] [SerializeField] private string aliasToPlay;
    public void PlayAlias()
    {
        transform.PlaySoundAtPosition(aliasToPlay);
    }
}
