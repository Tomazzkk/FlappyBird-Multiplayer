using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkyScript : NetworkBehaviour
{
    [SerializeField]
    Material Texture;
    float gamespeed;

    // Update is called once per frame
    void Update()
    {
        gamespeed = Time.deltaTime * 1;

        float speedSky = 0.1f;
        Texture.mainTextureOffset = Texture.mainTextureOffset + new Vector2(speedSky * gamespeed, 0);
    }
}
