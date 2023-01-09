using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevivePlayerPanel : MonoBehaviour
{
    private PlayerInstance _playerInstance;
    private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;
    private float _amountFiled = 0;
    public PlayerInstance PlayerInstance
    {
        get => _playerInstance;
        set => _playerInstance = value;
    }
    // Start is called before the first frame update
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInstance != null && _playerInstance.IsReviving)
        {
            transform.position = Camera.main.WorldToScreenPoint(PlayerInstance.transform.position);
            _canvasGroup.alpha = 1;
            _image.fillAmount = _amountFiled;
            _amountFiled += Time.deltaTime/3f;
        }
        else
        {
            _canvasGroup.alpha = 0;
            _amountFiled = 0;
        }
        
    }
}