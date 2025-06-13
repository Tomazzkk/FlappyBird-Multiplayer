using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] float initialDelay = 7f;
    [SerializeField] float cooldown = 2f;

    private float clock;

    private void Start()
    {
        clock = initialDelay; // começa com 7 segundos de espera
    }

    private void Update()
    {
        clock -= Time.deltaTime;

        if (clock <= 0f)
        {
            SpawnObstacle();
            clock = cooldown; // reseta o cooldown para o próximo spawn
        }
    }
   
    void SpawnObstacle()
    {
        if (IsServer)
        {
            Vector2 spawnPosition = new Vector2(GameManager.instance.ScreenBounds.x, Random.Range(-2f, 2f));
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            obstacle.GetComponent<NetworkObject>().Spawn();
            Debug.Log("FuncaoSpawmChamada");
        }
        
       
    }
}