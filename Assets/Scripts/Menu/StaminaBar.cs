using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void SetStamina(float current, float max)
    {
        Debug.Log($"Setting stamina bar: {current}/{max}");
        fill.fillAmount = current / max;
    }
}
