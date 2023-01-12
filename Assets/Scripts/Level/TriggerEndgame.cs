using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndgame : MonoBehaviour
{
    
    #region Events
    public delegate void EventHandler();
    public static event EventHandler eventTriggerEndgameStart;
    #endregion
    private List<PlayerInstance> playersInTrigger = new List<PlayerInstance>();

    [SerializeField] private GameObject viewModel;

    private Vector3 triggerInitialPosition;

    [SerializeField] private Transform _transformDestination;

    [SerializeField] private float EndgameDuration = 3;
    private float _currentTimer;
    [SerializeField] private GameObject particleSystemCameraTransition;

    private bool endgameIsRunning;
    public bool EndgameIsCompleted;
    
    // Start is called before the first frame update
    void Awake()
    {
        triggerInitialPosition = transform.position;
        particleSystemCameraTransition.SetActive(true);
        particleSystemCameraTransition.GetComponent<ParticleSystem>().Stop();
    }

    private bool AllPlayersAreInEndgameTrigger()
    {
        bool condition = true;
        if (LevelManager.Instance.players.Count == 0)
        {
            return false;
        }
        foreach (var player in LevelManager.Instance.players)
        {
            if (!playersInTrigger.Contains(player))
            {
                condition = false;
            }
        }
        return condition;
    }
    // Update is called once per frame
    void Update()
    {
        if (AllPlayersAreInEndgameTrigger() && !EndgameIsCompleted)
        {
            LevelManager.Instance.State = State.Intermission;
            
            Debug.Log("Players are in endgameTrigger");
            foreach (var player in LevelManager.Instance.players)
            {
                player.transform.parent = transform;
                player.SetSleep(true);
            }

            transform.Translate(Vector3.right * Time.deltaTime * 5);
            particleSystemCameraTransition.GetComponent<ParticleSystem>().Play();
            if (!endgameIsRunning)
            {
                eventTriggerEndgameStart?.Invoke();
                foreach (var ai in AIInstance.AIInstances)
                {
                    Destroy(ai.gameObject);
                }
                _currentTimer = EndgameDuration;

            }
            endgameIsRunning = true;
        }

        if (endgameIsRunning && !EndgameIsCompleted)
        {
            if (_currentTimer < 0 )
            {
                EndgameIsCompleted = true;
            }

            if (_currentTimer >= 0  )
                _currentTimer -= Time.deltaTime;
        }
    
    }

    private void AddPlayerToList(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            var playerInstance = other.transform.parent.GetComponent<PlayerInstance>();
            if (!playersInTrigger.Contains(playerInstance))
            {
                playersInTrigger.Add(playerInstance);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        AddPlayerToList(other);
    }
    private void OnTriggerEnter(Collider other)
    {
        AddPlayerToList(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            var playerInstance = other.transform.parent.GetComponent<PlayerInstance>();
            if (playersInTrigger.Contains(playerInstance))
            {
                playersInTrigger.Remove(playerInstance);
            }
        }
    }

    public void ResetTrigger()
    {
        foreach (var player in playersInTrigger)
        {
            player.SetSleep(false);
            player.transform.parent = null;
            playersInTrigger.Remove(player);
        }
        transform.position = triggerInitialPosition;
        particleSystemCameraTransition.GetComponent<ParticleSystem>().Stop();
        EndgameIsCompleted = false;
        endgameIsRunning = false;
    }
}
