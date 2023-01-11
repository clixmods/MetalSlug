using System;
using System.Collections.Generic;
using System.Linq;
using AudioAliase;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public enum State
{
    Ingame,
    Intermission
}

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
    public delegate void EventHandler();
    public delegate void EventHandlerRound(int newRound);
    public static event EventHandler eventPreLevelRestart;
    public static event EventHandler eventPostLevelRestart;
    public static event EventHandlerRound CallbackOnRoundChange;

    

    #endregion
    
    
    public List<PlayerInstance> players = new List<PlayerInstance>();
    [SerializeField] private Transform playerSpawnPoint;
    private RoundManager[] roundvolumes;
    [SerializeField] private int respawnAmount;
    [SerializeField] private int reviveAmount;
    [SerializeField] private int _currentRound = 0;
    private TriggerEndgame _triggerEndgame;
    [Header("Aliases")]
    [SerializeField][Aliase] private string RoundIntro;
    [SerializeField][Aliase] private string RoundStart;
    [SerializeField][Aliase] private string RoundEnd;
    [SerializeField][Aliase] private string RoundBossStart;
    [SerializeField][Aliase] private string RoundBossEnd;
    [SerializeField][Aliase] private string Gameover;
    public State State { get; set; }
    public int CurrentRound
    {
        get => _currentRound;
       private set
        {
            _currentRound = value;
            CallbackOnRoundChange?.Invoke(_currentRound);
        }
        
    }

    public int RespawnAmount
    {
        get => respawnAmount;
        set
        {
            if(value == 0)
                PlayerInputManager.instance.DisableJoining();
            else
            {
                PlayerInputManager.instance.EnableJoining();
            }
            respawnAmount = value;
            
        }
    }
    public int ReviveAmount
    {
        get => reviveAmount;
        set
        {
            respawnAmount = value;
        }
    }
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
        _triggerEndgame = FindObjectOfType<TriggerEndgame>();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        players = FindObjectsOfType<PlayerInstance>().ToList();
        PlayerInstance.eventPlayerJoin += AddPlayer;
        PlayerInstance.eventPlayerDisconnect += RemovePlayer;
        PlayerInstance.eventPlayerRespawn += RemoveRespawnAmount;
        AudioManager.PlaySoundAtPosition(RoundIntro, Vector3.zero);
        TriggerEndgame.eventTriggerEndgameStart += TriggerEndgameOneventTriggerEndgameStart;
    }

    private void TriggerEndgameOneventTriggerEndgameStart()
    {
        AudioManager.PlaySoundAtPosition(RoundEnd);
    }

    private void RemoveRespawnAmount(PlayerInstance newplayer)
    {
        RespawnAmount--;
    }


    private void Update()
    {
        if (_triggerEndgame.EndgameIsCompleted) 
        {
           _triggerEndgame.ResetTrigger();
           StartNewRound();
        }
    }

    private void StartNewRound()
    {
        LevelManager.Instance.State = State.Ingame;
        foreach (var player in players)
        {
            player.Teleport( playerSpawnPoint.position);
        }

        for (int i = 0; i < roundvolumes.Length; i++)
        {
            //roundvolumes[i].gameObject.SetActive(true);
        }
        eventPreLevelRestart?.Invoke();
        eventPostLevelRestart?.Invoke();
        AudioManager.PlaySoundAtPosition(RoundStart,Vector3.zero);
        CurrentRound++;
    }
}
