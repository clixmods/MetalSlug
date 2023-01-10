using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerAmmo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textAmmo;
    [SerializeField] private TextMeshProUGUI textGrenade;
    
    public void SetAmmo(int amount)
    {
        if (amount == -1)
        {
            textAmmo.text = "∞";
        }
        else
        {
            textAmmo.text = amount.ToString();
        }
    }

    public void SetGrenade(int amount)
    {
        if (amount == -1)
        {
            textGrenade.text =  "∞";
        }
        else
        {
            textGrenade.text = amount.ToString();
        }
    }
}
