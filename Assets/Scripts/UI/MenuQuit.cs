using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuQuit : MonoBehaviour
{
    CameraShake cameraShake;
    public UnityEvent onCompleteCallBack;

    public void OnEnable()
    {
        transform.localScale = new Vector3(0,0,0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.3f).setDelay(0.1f).setOnComplete(OnComplete);
    }

    public void OnComplete()
    {
        if (onCompleteCallBack != null)
        {
            onCompleteCallBack.Invoke();
        }
    }

    public void OnClose()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.5f).setOnComplete(DestroyMe);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}