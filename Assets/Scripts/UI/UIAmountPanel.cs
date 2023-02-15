using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIAmountPanel : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_textMeshProUGUI.text == "0")
        {
            _textMeshProUGUI.color = Color.red;
        }
        else
        {
            _textMeshProUGUI.color = Color.white;
        }
    }
}
