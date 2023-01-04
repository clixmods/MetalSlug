using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinAndLeftPlayer : MonoBehaviour
{
    [SerializeField] CinemachineTargetGroup targetbrain;

    private void Start()
    {
        targetbrain = this.gameObject.GetComponent<CinemachineTargetGroup>();
        PlayerInstance.eventPlayerJoin += AddPlayer;
        PlayerInstance.eventPlayerDisconnect += RemovePlayer;
    }
    public void AddPlayer(PlayerInstance player)
    {
        targetbrain.AddMember(player.transform, 1f, 0f);
    }
    public void RemovePlayer(PlayerInstance player)
    {
        targetbrain.RemoveMember(player.transform);
    }
}
