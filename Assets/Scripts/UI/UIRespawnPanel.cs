using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIRespawnPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (textComponent != null)
        {
            textComponent.text = LevelManager.Instance.RespawnAmount.ToString();
        }
    }
}
