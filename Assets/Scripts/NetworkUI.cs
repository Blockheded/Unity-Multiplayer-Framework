using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI playersCountText;
    bool serverAwake = true;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        NetworkManager.Singleton.OnServerStopped += serverOff;
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    private void Update()
    {
        playersCountText.text = "Players: " + playersNum.Value.ToString();

        if (!IsServer||!serverAwake) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    private void serverOff(bool obj)
    {
        serverAwake = false;
    }
}
