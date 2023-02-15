using UnityEngine;
using TMPro;

public class UIPressJoin : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private Animator _animator;
    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        LevelManager.eventRespawnPointUsed += PlayerInstanceOneventPlayerSpawn;
    }

    private void PlayerInstanceOneventPlayerSpawn()
    {
        if (LevelManager.Instance.RespawnAmount > 0)
        {
            _textMeshProUGUI.text = "Press start to join\nCost [1] Respawn point";
            _animator.enabled = true;
        }
        else
        {
            _textMeshProUGUI.text = "No respawn available";
            _textMeshProUGUI.color = Color.red;
            _animator.enabled = false;
        }
    }
}
