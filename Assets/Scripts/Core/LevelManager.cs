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

    #region Events
    public delegate void CallbackOnLevelRestartLoop();
    public static event CallbackOnLevelRestartLoop eventLevelRestartLoop;
    

    #endregion
    
    
    private List<PlayerInstance> players = new List<PlayerInstance>();
    [SerializeField] private Transform playerSpawnPoint;
    private RoundManager[] roundvolumes;
    
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
            if (Instance.players.Count > 1)
            {
                player.Teleport(Instance.players[0].transform.position);
            }
        }
    }
    public static void RemovePlayer(PlayerInstance player)
    {
        if (Instance.players.Contains(player))
        {
            Instance.players.Remove(player);
        }
    }

    private void Awake()
    {
        roundvolumes = FindObjectsOfType<RoundManager>();
    }

    private void Start()
    {
        players = FindObjectsOfType<PlayerInstance>().ToList();
        PlayerInstance.eventPlayerJoin += AddPlayer;
        PlayerInstance.eventPlayerDisconnect += RemovePlayer;
    }

    private void Update()
    {
        bool allRoundsClean = true;
        
            for (int i = 0; i < roundvolumes.Length; i++)
            {
                if (roundvolumes[i].gameObject.activeSelf)
                {
                    allRoundsClean = false;
                }
            }
            // stay AI on the map
            if(AIInstance.AIInstances.Count > 0)
                allRoundsClean = false;

            if (allRoundsClean)
            {
                foreach (var player in players)
                {
                    player.Teleport( playerSpawnPoint.position);
                }

                for (int i = 0; i < roundvolumes.Length; i++)
                {
                    roundvolumes[i].gameObject.SetActive(true);
                }
                eventLevelRestartLoop?.Invoke();
            }
    }
}
