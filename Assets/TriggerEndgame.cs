using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndgame : MonoBehaviour
{
    private List<PlayerInstance> playersInTrigger = new List<PlayerInstance>();

    [SerializeField] private GameObject viewModel;

    private Vector3 triggerInitialPosition;

    [SerializeField] private Transform _transformDestination;

    [SerializeField] private float durationToGoToDestination = 10f;

    [SerializeField] private GameObject particleSystemCameraTransition;

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
        if (AllPlayersAreInEndgameTrigger())
        {
            Debug.Log("Players are in endgameTrigger");
            foreach (var player in LevelManager.Instance.players)
            {
                player.transform.parent = transform;
            }

            transform.LeanMove(_transformDestination.position , durationToGoToDestination);
            particleSystemCameraTransition.GetComponent<ParticleSystem>().Play();
        }

        if (Vector3.Distance(transform.position, _transformDestination.position) < 1)
        {
            EndgameIsCompleted = true;
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
            playersInTrigger.Remove(player);
        }

        transform.position = triggerInitialPosition;
        particleSystemCameraTransition.GetComponent<ParticleSystem>().Stop();
    }
}
