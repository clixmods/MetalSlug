using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class FirstSelectALetter : MonoBehaviour
{
    private PlayerInputs _input;
    /// <summary>
    /// Input used by the player
    /// </summary>
    public PlayerInputs Input
    {
        get
        {
            // Prevent null ref when the game reload script
            if (_input == null)
            {
                _input = new PlayerInputs();
            }
            return _input;
        }
    }
    private void OnEnable()
    {
        Input.PlayerUI.Enable();
    }
    private void OnDisable()
    {
        Input.PlayerUI.Disable();
    }
}