using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
using JetBrains.Annotations;
using System;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
   public Lobby hostLobby, joinedLobby;
    public TextMeshProUGUI textoCodigo;
    public TMP_InputField textoInseriCod;
    public TextMeshProUGUI textoNome;
    public GameObject panelInicio;
    public GameObject panelSala;
    public TMP_InputField nomes;
    public TMP_Text[] lobbyNameList;
    public GameObject BotaoStart;
    public bool iniciouJogo;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await LoginAnonimo();
    }

    async Task LoginAnonimo()
    {


        if (AuthenticationService.Instance.IsSignedIn)
        {
            return;
        }


        AuthenticationService.Instance.ClearSessionToken();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        // textoNome.text = "O seu login é:  " + AuthenticationService.Instance.PlayerId;


    }

    async Task CriaSala()
    {

        try
        {
            CreateLobbyOptions LobbyName = new CreateLobbyOptions
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"StartGame", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }


            };
            hostLobby = await LobbyService.Instance.CreateLobbyAsync("NomeDoLobby", 4, LobbyName);
            joinedLobby = hostLobby;
            textoCodigo.text = hostLobby.LobbyCode;
            InvokeRepeating("EnviaPing", 10, 10);
            panelInicio.SetActive(false);
            panelSala.SetActive(true);
            BotaoStart.SetActive(true);
            InvokeRepeating("VerificaUpdate", 3, 3);


        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            if (e.ErrorCode == 16004)
            {
                Debug.Log("O lobby está cheio!");
            }

        }


    }

    public void MostraPlayers()
    {
        if (joinedLobby == null || joinedLobby.Players == null)
            return;
        for (int i = 0; i < joinedLobby.Players.Count; i++)
        {
            lobbyNameList[i].text = joinedLobby.Players[i].Data["Nome"].Value;
        }
    }

    public void VerificaUpdate()
    {
        if (joinedLobby == null)
            return;

        if (joinedLobby.Data != null && joinedLobby.Data.ContainsKey("StartGame") && joinedLobby.Data["StartGame"].Value != "0")
        {
            EntraRelay(joinedLobby.Data["StartGame"].Value);
        }

        AtualizaLobby();
        MostraPlayers();
    }

    public async void AtualizaLobby()
    {
        if (joinedLobby == null)
            return;

        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
    }

    public async void BotaoCria()
    {
        await CriaSala();
    }

    

    public async Task EntrarSala()
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                JoinLobbyByCodeOptions LobbyOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };

                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(textoInseriCod.text, LobbyOptions);
                panelInicio.SetActive(false);
                panelSala.SetActive(true);
                textoCodigo.text =joinedLobby.LobbyCode;

                InvokeRepeating("VerificaUpdate", 3, 3);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            if (e.ErrorCode == 16010)
            {
                Debug.Log("Codigo Invalido");
            }
        }

    }

    public async void BotaoEntra()
    {
        await EntrarSala();
    }

    public async void EnviaPing()
    {
        if (hostLobby != null)
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            Debug.Log("Enviou Ping");
        }
    }

    async Task<String> CriarRelay()
    {

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

        string codigoAloc = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        return codigoAloc;
    }


    async void EntraRelay(string codigoDeAlocacao)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(codigoDeAlocacao);

        RelayServerData relayServiceData = new RelayServerData(joinAllocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServiceData);

        NetworkManager.Singleton.StartClient();

        CancelInvoke("verificaUpdate");
    }

    public async void IniciaJogo()
    {
        if (joinedLobby == null)
            return;

        string codigorelay = await CriarRelay();

        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { "StartGame", new DataObject(DataObject.VisibilityOptions.Member, codigorelay) }
            }
        });

        joinedLobby = lobby;

        CancelInvoke("VerificaUpdate");
        CancelInvoke("EnviaPing");
        panelSala.SetActive(false);
        iniciouJogo = true;

         await Task.Delay(7000);
        NetworkManager.Singleton.SceneManager.LoadScene("Jogo", LoadSceneMode.Single);
    }

    Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
        {
            { "Nome", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, nomes.text) }
        }
        };
    }
}