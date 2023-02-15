using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class UIControllerGuide : MonoBehaviour
{
   [Serializable]
    struct PanelController
    {
        public string name;
        public GameObject UIRelatedToController;
        public InputControlScheme _inputControlScheme;

        // PanelController()
        // {
        //     name = _inputControlScheme.name;
        // }
    }
    public InputActionAsset playerInputs;
    [SerializeField] private PanelController[] panelsController;
    private PlayerInput _input;
    [SerializeField] private Slider _slider;
    [SerializeField] private float delayBetweenChangeInfoController;
    [SerializeField] private GameObject virtualKeyboard;
    private float _currentTimer;
    private int _currentIndex;

    private void OnValidate()
    {
        for (int i = 0; i < panelsController.Length; i++)
        {
            panelsController[i].name = playerInputs.controlSchemes.ToArray()[i].name;
            panelsController[i]._inputControlScheme = playerInputs.controlSchemes.ToArray()[i];
        }
    }
    void OnButtonPressed(InputControl button)
    {
        for (int i = 0; i < panelsController.Length; i++)
        {
            if (!panelsController[i]._inputControlScheme.SupportsDevice(button.device))
            {
                panelsController[i].UIRelatedToController.SetActive(false);
            }
            else
            {
                _currentIndex = i;
                _currentTimer = 0;
                panelsController[i].UIRelatedToController.SetActive(true);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInput();
        _input.onControlsChanged += InputOnonControlsChanged;
        InputSystem.onAnyButtonPress.Call(OnButtonPressed);

        InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        Debug.Log(device.displayName);
                        // New Device.
                        break;
                    case InputDeviceChange.Disconnected:
                        // Device got unplugged.
                        break;
                    case InputDeviceChange.Reconnected:
                        // Plugged back in.
                        break;
                    case InputDeviceChange.Removed:
                        // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                        break;
                    default:
                        // See InputDeviceChange reference for other event types.
                        break;
                }
            };
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (virtualKeyboard.activeSelf)
        {
            for (int i = 0; i < panelsController.Length; i++)
            {
                panelsController[i].UIRelatedToController.SetActive(false);
            }
            _slider.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (!_slider.gameObject.activeSelf)
            {
                _currentTimer = 0;
                panelsController[_currentIndex].UIRelatedToController.SetActive(true);
                _slider.gameObject.SetActive(true);
            }
            
        }
        

        if (_currentTimer <= delayBetweenChangeInfoController)
        {
            _slider.value = _currentTimer / delayBetweenChangeInfoController;
            _currentTimer += Time.deltaTime;
        }
        else
        {
            _currentIndex++;
            if (_currentIndex < panelsController.Length)
            {
                _currentTimer = 0;
                for (int i = 0; i < panelsController.Length; i++)
                {
                    panelsController[i].UIRelatedToController.SetActive(false);
                }
                panelsController[_currentIndex].UIRelatedToController.SetActive(true);
            }
            else
            {
                _currentIndex = 0;
            }
        }
    }
    private void InputOnonControlsChanged(PlayerInput obj)
    {
        Debug.Log(obj.currentControlScheme);
        for (int i = 0; i < panelsController.Length; i++)
        {
            if (panelsController[i].name != obj.currentControlScheme)
            {
                panelsController[i].UIRelatedToController.SetActive(false);
            }
            else
            {
                _currentIndex = i;
                _currentTimer = 0;
                panelsController[i].UIRelatedToController.SetActive(true);
            }
                
        }
    }

}
