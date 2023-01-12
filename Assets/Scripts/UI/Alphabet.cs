using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Alphabet : MonoBehaviour
{
    [SerializeField] HighscoreTable highscoreTable;
    [SerializeField] GameObject transformTest;
    private bool delPressed = false;
    private TextMeshProUGUI textMeshProUGUI;
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
    }

    public void OnClickLetter(string letter)
    {
        if (nameTapped.Length >= 3)
        { return; }
        nameTapped += letter;
    }
}
