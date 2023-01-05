using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelPlayer : MonoBehaviour
{
    private static List<PlayerInstance> _playerInstancesLinked = new List<PlayerInstance>();
    private PlayerInstance _playerInstance;

    [SerializeField] private GameObject panelTextJoin;
    [SerializeField] private GameObject panelPlayer;

    [SerializeField]
    private bool isHostPanel;
    private void Awake()
    {
        panelTextJoin.SetActive(true);
        panelPlayer.SetActive(false);
        PlayerInstance.eventPlayerJoin += PlayerInstanceOneventPlayerJoin;
        PlayerInstance.eventPlayerDisconnect += PlayerInstanceOneventPlayerDisconnect;
    }

   

    private void PlayerInstanceOneventPlayerDisconnect(PlayerInstance newplayer)
    {
       
        if (_playerInstance == newplayer && _playerInstancesLinked.Contains(newplayer))
        {
            _playerInstance = null;
            panelTextJoin.SetActive(true);
            panelPlayer.SetActive(false);
            _playerInstancesLinked.Remove(newplayer);
        }
    }

    private void PlayerInstanceOneventPlayerJoin(PlayerInstance newplayer)
    {
        if (!isHostPanel && _playerInstancesLinked.Count == 0) return;
        if (_playerInstance == null && !_playerInstancesLinked.Contains(newplayer) )
        {
            _playerInstance = newplayer;
            panelTextJoin.SetActive(false);
            panelPlayer.SetActive(true);
            _playerInstancesLinked.Add(newplayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInstance == null)
        {
            
        }
        else
        {
            
        }
        
        
    }
}
