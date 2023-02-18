using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTitleScreen : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    
    private CameraMotor _cameraMotor;
    // Start is called before the first frame update
    void Start()
    {
        
        _cameraMotor = CameraUtility.Camera.transform.GetComponent<CameraMotor>();
        PlaceCameraInTramway();
        LevelManager.EventOnStateChanged += LevelManagerOnEventOnStateChanged;
    }

    private void LevelManagerOnEventOnStateChanged(State state)
    {
        switch (state)
        {
            case State.InGame:
                _cameraMotor.enabled = true;
                var cameraTransform = CameraUtility.Camera.transform;
                cameraTransform.SetParent(null);
                break;
            case State.Intermission:
                break;
            case State.GameOver:
                break;
            case State.Menu:
                PlaceCameraInTramway();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void PlaceCameraInTramway()
    {
        _cameraMotor.enabled = false;
        var cameraTransform = CameraUtility.Camera.transform;
        cameraTransform.SetParent(this.cameraTransform);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
