using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    #region Singleton

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if(_instance == null)
                    _instance = new GameObject("LevelManager").AddComponent<LevelManager>();
            }

            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    #endregion
    private List<PlayerInstance> players = new List<PlayerInstance>();


    public static void AddPlayer(PlayerInstance player)
    {
        if (!Instance.players.Contains(player))
        {
            Instance.players.Add(player);
        }
    }

    public static void RemovePlayer(PlayerInstance player)
    {
        if (Instance.players.Contains(player))
        {
            Instance.players.Remove(player);
        }
    }

    private void Start()
    {
        players = FindObjectsOfType<PlayerInstance>().ToList();
    }

    private void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players[i] == null)
                players.RemoveAt(i);
        }
        
    }
}
