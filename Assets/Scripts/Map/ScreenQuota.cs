using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScreenQuota : NetworkBehaviour
{
    [SerializeField] private TMP_Text quotaText;
    private NetworkVariable<int> currentQuota = new NetworkVariable<int>(5);

    // Start is called before the first frame update
    void Start()
    {
        currentQuota.OnValueChanged += (int previousValue, int newValue) =>
        {
            quotaText.text = newValue.ToString();
        };
    }

    [ServerRpc]
    public void IncreaseQuotaServerRpc(int amount)
    {
        currentQuota.Value += amount;
    }
}
