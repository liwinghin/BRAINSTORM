using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Unity.Netcode;
using TMPro;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI playersCountText;

    private NetworkVariable<int> playersCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Start()
    {
        hostButton.OnClickAsObservable().Subscribe(_=>
        {
            NetworkManager.Singleton.StartHost();
        }).AddTo(this);

        clientButton.OnClickAsObservable().Subscribe(_ =>
        {
            NetworkManager.Singleton.StartClient();
        }).AddTo(this);

        playersCount.OnValueChanged += (previousValue, newValue) =>
        {
            playersCountText.text = $"Players: {playersCount.Value}";
        };

        Observable.EveryUpdate()
            .Where(_ => IsServer)
            .Subscribe(_ => playersCount.Value = NetworkManager.Singleton.ConnectedClients.Count);

    }
}
