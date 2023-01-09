using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevivePlayerPanel : MonoBehaviour
{
    private PlayerInstance _playerInstance;
    private CanvasGroup _canvasGroup;
    public PlayerInstance PlayerInstance
    {
        get => _playerInstance;
        set => _playerInstance = value;
    }
    [SerializeField] private Image _image;
    // Start is called before the first frame update
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInstance.ctxCached > 0)
        {
            transform.position = Camera.main.WorldToScreenPoint(PlayerInstance.transform.position);
            _canvasGroup.alpha = 1;
            _image.fillAmount = _playerInstance.ctxCached / 3f;
        }
        else
        {
            _canvasGroup.alpha = 0;
        }
        
    }
}
