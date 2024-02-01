using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class Shader : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private List<Transform> points;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private int texSize = 256;
    
    private SpriteRenderer _spriteRenderer;
    public Texture2D texture2D;
    public Sprite sprite;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        renderTexture = new RenderTexture(texSize, texSize, texSize);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        computeShader.SetTexture(0,"Result" ,renderTexture);
        computeShader.SetFloat("texSize" ,texSize);
        
        computeShader.Dispatch(0, renderTexture.width/8, renderTexture.height/8, 1);
        
        texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        
        sprite = Sprite.Create(texture2D, new Rect(0,0, texSize,texSize), new Vector2(0.5f,0.5f), 100);

        _spriteRenderer.sprite = sprite;

    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetVector("point0" , new Vector4(points[0].position.x,points[0].position.y,points[0].position.z, 0));
        computeShader.SetVector("point1" , new Vector4(points[1].position.x,points[1].position.y,points[1].position.z, 0));
        computeShader.SetVector("point2" , new Vector4(points[2].position.x,points[2].position.y,points[2].position.z, 0));
        computeShader.SetVector("point3" , new Vector4(points[3].position.x,points[3].position.y,points[3].position.z, 0));
        
        computeShader.Dispatch(0, renderTexture.width/8, renderTexture.height/8, 1);
        
        UpdateTextureFromRenderTexture(renderTexture);
        
    }
    
    public void UpdateTextureFromRenderTexture(RenderTexture rTex)
    {
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = rTex;

        mainCam.Render();

        // Create a new Texture2D and read the RenderTexture image into it
        
        texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentActiveRT;
    }
}
