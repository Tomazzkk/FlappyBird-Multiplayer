using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Playername : NetworkBehaviour
{
    TMP_Text playerNameText;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        playerNameText = GetComponentInChildren<TMP_Text>();

        LobbyManager lobby = FindObjectOfType<LobbyManager>();

        if (IsServer)
        {
            while (NetworkManager.Singleton.ConnectedClients.Count != lobby.joinedLobby.Players.Count)
                yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
            {
                NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponentInChildren<Playername>().
                    SetPlayerNameClientRpc(lobby.joinedLobby.Players[i].Data["Nome"].Value);
            }
        }
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName)
    {
        playerNameText.text = playerName;
    }

  
}