using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIRevivePanel : MonoBehaviour
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
            textComponent.text = LevelManager.Instance.ReviveAmount.ToString();
        }
    }
}
