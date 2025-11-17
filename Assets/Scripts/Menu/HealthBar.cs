using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    
    public void SetHealth(float current, float max)
    {
        Debug.Log($"Setting health bar: {current}/{max}");
        fill.fillAmount = current / max;
    }
}
