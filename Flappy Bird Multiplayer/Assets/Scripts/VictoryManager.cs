using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class VictoryManager : NetworkBehaviour
{
    public static VictoryManager Instance;

    [SerializeField] public GameObject victoryPanel;
    [SerializeField] public GameObject defeatPanel;

    private NetworkList<ulong> alivePlayers = new NetworkList<ulong>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            alivePlayers.Clear(); // só o Host manipula
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                alivePlayers.Add(clientId);
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void ReportDeathServerRpc(ulong playerId)
    {
        if (alivePlayers.Contains(playerId))
            alivePlayers.Remove(playerId);

        if (alivePlayers.Count == 1)
        {
            ulong winnerId = alivePlayers[0];
            ShowVictoryClientRpc(winnerId);
        }
    }

    [ClientRpc]
    private void ShowVictoryClientRpc(ulong winnerId)
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;

        Debug.Log($"Sou o cliente {localId}. Vencedor é {winnerId}");

        if (localId == winnerId)
        {
            victoryPanel.SetActive(true);

        }
        else
        {
            defeatPanel.SetActive(true);
        }

        Time.timeScale = 0;
    }
}
