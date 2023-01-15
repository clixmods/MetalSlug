using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioAliase;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
public enum State
{
    InGame,
    Intermission,
    GameOver,
    Menu
}
public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool conditionTimer = false;
    [SerializeField] private TextMeshProUGUI timerUGUI;
    [SerializeField] private float currentValue;
    [SerializeField] private GameObject textMeshProUGUI;
    [SerializeField] private GameObject menuCanvas;
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
    public static event EventHandler eventSessionStart;
    public static event EventHandler eventResetSession;
    public static event EventHandler eventEndgame;
    
    #endregion
    private List<PlayerInstance> players = new List<PlayerInstance>();
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private int respawnAmount;
    [SerializeField] private int reviveAmount;
    [SerializeField] private int _currentRound = 0;
    private TriggerEndgame _triggerEndgame;
    private bool _firstSpawn;
    private bool _secondFirstSpawn;
    [Header("Aliases")]
    [SerializeField][Aliase] private string RoundIntro;
    [SerializeField][Aliase] private string RoundStart;
    [SerializeField][Aliase] private string RoundEnd;
    [SerializeField][Aliase] private string Gameover;
    private State _state = State.Menu;
    public State State { get => _state; set => _state = value; }
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
            if(value > 0)
                PlayerInputManager.instance.EnableJoining();
            else
            {
                PlayerInputManager.instance.DisableJoining();
            }
            respawnAmount = value;
        }
    }
    public int ReviveAmount
    {
        get => reviveAmount;
        set
        {
            reviveAmount = value;
        }
    }
    #region Properties
    /// <summary>
    /// Get a random alive player from alive players
    /// </summary>
    public static PlayerInstance GetRandomAlivePlayers
    {
        get
        {
            List<PlayerInstance> playersAlives = GetAlivePlayers;
            for (int i = 0; i < playersAlives.Count; i++)
            {
                var player = playersAlives[i];
                // The last player of the list ? return it
                if (i == playersAlives.Count - 1)
                {
                    return player;
                } 
                if (0.5f < Random.Range(0f, 1f))
                {
                    return player;
                }
            }
            return null;
        }
    }
    /// <summary>
    /// Return a list of alive players, laststand player are not included.
    /// </summary>
    public static List<PlayerInstance> GetAlivePlayers
    {
        get
        {
            List<PlayerInstance> playersAlive = new List<PlayerInstance>();
            for (int i = 0; i < Instance.players.Count; i++)
            {
                var player = Instance.players[i];
                // TODO : check expensive, players list are not supposed to have null element in it.
                if (player == null) continue;
                if (player.IsAlive)
                {
                    playersAlive.Add(player);
                }
            }
            return playersAlive;
        }
    }

    /// <summary>
    /// Return a list of players in the game
    /// </summary>
    public static List<PlayerInstance> Players => Instance.players;
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
            // Start the game if we are in menu
            if(Instance.State == State.Menu && Instance.players.Count <= 1)
            {
                _instance.conditionTimer = true;
                _instance.menuCanvas.SetActive(false);
                Debug.Log("Game is starting...");
                _instance.timerUGUI.gameObject.SetActive(true);
                _instance.currentValue = 3;
                Instance.StartCoroutine(Instance.CoolDownBeforeStart());
                
            }
        }
    }
    public static void RemovePlayer(PlayerInstance player)
    {
        if (Instance.players.Contains(player))
        {
            Instance.players.Remove(player);
        }

        if (GetAlivePlayers.Count == 0)
        {
            foreach (var ai in AIInstance.AIInstances)
            {
                if(!ai.Sleep)
                    ai.OnDown();
            }
        }
    }

    private void Awake()
    {
        _triggerEndgame = FindObjectOfType<TriggerEndgame>();
        eventSessionStart += SessionIntro;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        players = FindObjectsOfType<PlayerInstance>().ToList();
        PlayerInstance.eventPlayerJoin += AddPlayer;
        PlayerInstance.eventPlayerDeath += RemovePlayer;
        PlayerInstance.eventPlayerSpawn += RemoveRespawnAmount;
        PlayerInstance.eventPlayerRevive += PlayerInstanceOneventPlayerRevive;
        TriggerEndgame.eventTriggerEndgameStart += TriggerEndgameOneventTriggerEndgameStart;
    }

    public static void ResetSession()
    {
        Instance.State = State.Menu;
        eventResetSession?.Invoke(); 
        Instance.respawnAmount = 3;
        Instance.reviveAmount = 3;
        Instance._firstSpawn = false;
        Instance._secondFirstSpawn = false;
        Instance._currentRound = 1;
        
        PlayerInputManager.instance.EnableJoining();

    }

    void SessionIntro()
    {
        AudioManager.PlaySoundAtPosition(RoundIntro, Vector3.zero);

    }
    IEnumerator CoolDownBeforeStart()
    {
        textMeshProUGUI.SetActive(true);
        yield return new WaitForSeconds(3);
        _instance.conditionTimer = false;
        _instance.timerUGUI.gameObject.SetActive(false);
        textMeshProUGUI.SetActive(false);

        State = State.InGame;
        eventSessionStart?.Invoke();
    }

    private void PlayerInstanceOneventPlayerRevive(PlayerInstance newplayer)
    {
        ReviveAmount--;
    }

    private void TriggerEndgameOneventTriggerEndgameStart()
    {
        AudioManager.PlaySoundAtPosition(RoundEnd);
    }

    
    private void RemoveRespawnAmount(PlayerInstance newplayer)
    {
        if (_currentRound == 1 && !_firstSpawn)
        {
            _firstSpawn = true;
            return;
        }
        if (_currentRound == 1 && players.Count > 1 && !_secondFirstSpawn)
        {
            _secondFirstSpawn = true;
            return;
        }
        RespawnAmount--;
    }


    private void Update()
    {
        if (conditionTimer)
        {
            _instance.currentValue -= 1 * Time.deltaTime;
            _instance.timerUGUI.text = currentValue.ToString("0");
        }

        if (_triggerEndgame.EndgameIsCompleted) 
        {
           _triggerEndgame.ResetTrigger();
           StartNewRound();
        }

        if (GetAlivePlayers.Count == 0 && RespawnAmount <= 0 && State == State.InGame)
        {
            State = State.GameOver;
            Debug.Log("ENDGAME");
            eventEndgame?.Invoke();
            // Execute endgame function here
        }
    }

    private void StartNewRound()
    {
        Instance.State = State.InGame;
        foreach (var player in players)
        {
            player.Teleport( playerSpawnPoint.position);
        }
        eventPreLevelRestart?.Invoke();
        eventPostLevelRestart?.Invoke();
        AudioManager.PlaySoundAtPosition(RoundStart,Vector3.zero);
        CurrentRound++;
    }
}
