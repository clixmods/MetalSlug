using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.UI;

public class Alphabet : MonoBehaviour
{
    public Action<string> action;
    [SerializeField] HighscoreTable highscoreTable;
    [SerializeField] GameObject transformTest;
    public TextMeshProUGUI textMeshProUGUI;
    public Text text;
    private TextMeshProUGUI savedName;
    public string nameTapped = "";

    private void Start()
    {
        textMeshProUGUI = transformTest.GetComponent<TextMeshProUGUI>();
        savedName = textMeshProUGUI;
    }

    void Update()
    {
        textMeshProUGUI.text = nameTapped;
        if(text != null)
        {
            text.text = nameTapped;
        }
    }

    public void OnClickLetter(string letter)
    {
        if (nameTapped.Length >= 3)
        { return; }
        nameTapped += letter;
    }

    public void OnPressedEnd()
    {
        action?.Invoke(nameTapped);
        nameTapped = "";
        textMeshProUGUI.text = nameTapped;
    }
}