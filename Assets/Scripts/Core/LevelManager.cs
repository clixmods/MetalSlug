using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


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

    #region Properties

    public static PlayerInstance GetRandomAlivePlayers
    {
        get
        {
            for (int i = 0; i < Instance.players.Count; i++)
            {
                var player = Instance.players[i];
                if (player == null) continue;
                if (player.IsAlive)
                {
                    if (i == Instance.players.Count - 1)
                    {
                        if (player.IsAlive)
                        {
                            return player;
                        }
                    }
                    else if (0.5f < Random.Range(0f, 1f))
                    {
                        return player;
                    }
                }
            }

            return null;
        }
    }

    public static List<PlayerInstance> GetAlivePlayers
    {
        get
        {
            List<PlayerInstance> playersAlive = new List<PlayerInstance>();
            for (int i = 0; i < Instance.players.Count; i++)
            {
                var player = Instance.players[i];
                if (player == null) continue;
               
                
                if (player.IsAlive)
                {
                    playersAlive.Add(player);
                }
            }

            return playersAlive;
        }
    }

    #endregion
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
        PlayerInstance.eventPlayerJoin += AddPlayer;
        PlayerInstance.eventPlayerDisconnect += RemovePlayer;
    }
}
