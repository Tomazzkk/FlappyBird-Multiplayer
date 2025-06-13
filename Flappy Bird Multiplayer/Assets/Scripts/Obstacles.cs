using Unity.Netcode;
using UnityEngine;

public class Obstacles : NetworkBehaviour
{
    float speed = 3.5f;
    public static bool Inicia = false;
    public static Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
       

    }

    private void Update()
    {
        Invoke("MudaGravidade", 7);
        if (Inicia)
        {
            if (transform.position.x < -GameManager.instance.ScreenBounds.x)
            {
                Destroy(gameObject);
            }
            rigidbody2D.velocity = Vector2.left * speed;
        }


    }
    public void MudaGravidade()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.gravityScale = 0f;
    }
}