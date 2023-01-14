using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelPlayer : MonoBehaviour
{
    private static List<PlayerInstance> _playerInstancesLinked = new List<PlayerInstance>();
    private PlayerInstance _playerInstance;

    [SerializeField] private UIPlayerAmmo _uiPlayerAmmo;
    [SerializeField] private UIRevivePlayerPanel _uiPlayerRevive;
    [SerializeField] private GameObject panelTextJoin;
    [SerializeField] private GameObject panelPlayer;
    [SerializeField] private bool isHostPanel;
    private void Awake()
    {
        panelTextJoin.SetActive(true);
        panelPlayer.SetActive(false);
        _uiPlayerAmmo ??= GetComponentInChildren<UIPlayerAmmo>();
        PlayerInstance.eventPlayerJoin += PlayerInstanceOneventPlayerJoin;
        PlayerInstance.eventPlayerDeath += PlayerInstanceOneventPlayerDeath;
        PlayerInstance.eventPlayerFire += PlayerInstanceOneventPlayerFire;
    }

    private void PlayerInstanceOneventPlayerFire(PlayerInstance newplayer)
    {
        if (_playerInstance == newplayer  && _playerInstancesLinked.Contains(newplayer))
        {
            _uiPlayerAmmo.SetAmmo(_playerInstance.weaponInstance.CurrentAmmo);
            _uiPlayerAmmo.SetGrenade(_playerInstance.grenadeInstance.CurrentAmmo);

        }
    }

    private void PlayerInstanceOneventPlayerDeath(PlayerInstance newplayer)
    {
       
        if (_playerInstance == newplayer && _playerInstancesLinked.Contains(newplayer))
        {
            _playerInstance = null;
            panelTextJoin.SetActive(true);
            panelPlayer.SetActive(false);
            _playerInstancesLinked.Remove(newplayer);
            _uiPlayerRevive.PlayerInstance = null;
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
            _uiPlayerRevive.PlayerInstance = newplayer;
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
