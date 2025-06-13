using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Player : NetworkBehaviour
{
    const float jumpForce = 6;
    GameObject GameOverPanel;
    GameObject WinPanel;
    Rigidbody2D rigidbody2D;

    public Dictionary<string, PlayerDataObject> Data { get; internal set; }

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            rigidbody2D.velocity = Vector3.zero;
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameOver();
            
                    
        }
        else if (collision.gameObject.CompareTag("Score"))
        {
            GameManager.instance.Score++;
            UIManager.instance.UpdateScoreText();
        }
    }

    void GameOver()
    {
        if (IsOwner)
        {
            VictoryManager.Instance.ReportDeathServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        if (GameManager.instance != null && PlayerPrefs.GetInt("Record") < GameManager.instance.Score)
        {
            PlayerPrefs.SetInt("Record", GameManager.instance.Score);
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.GameOver();
        }
        else
        {
            Debug.LogWarning("UIManager não encontrado!");
        }
    }

    public override void OnNetworkSpawn()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;

        Invoke("IniciaGame", 7);
        base.OnNetworkSpawn();
    }
    public void IniciaGame()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        // Physics.gravity = new Vector3(0, -9.81f, 0);
        rigidbody2D.gravityScale = 1;
        //Obstacles.rigidbody2D.gravityScale = 1;
        Obstacles.Inicia = true;
        Debug.Log("Jogo iniciado");
    }

}