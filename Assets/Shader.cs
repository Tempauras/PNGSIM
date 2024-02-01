using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteMask))]
public class Shader : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private List<Transform> points;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private int texSize = 256;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float scale;
    [SerializeField] private float tolerance;
    
    
    
    private SpriteMask _spriteRenderer;
    public Texture2D texture2D;
    public Sprite sprite;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteMask>();

        renderTexture = new RenderTexture(texSize, texSize, texSize);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        computeShader.SetTexture(0,"Result" ,renderTexture);
        computeShader.SetFloat("texSize" ,texSize);
        
        computeShader.Dispatch(0, renderTexture.width/8, renderTexture.height/8, 1);
        
        texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        texture2D.filterMode = FilterMode.Point;
        
        sprite = Sprite.Create(texture2D, new Rect(0,0, texSize,texSize), new Vector2(0.5f,0.5f), 100);

        _spriteRenderer.sprite = sprite;

    }

    // Update is called once per frame
    void Update()
    {
        // computeShader.SetVector("point0" , new Vector4(points[0].position.x*scale+offset.x,points[0].position.y*scale+offset.y,points[0].position.z, 0));
        // computeShader.SetVector("point1" , new Vector4(points[1].position.x*scale+offset.x,points[1].position.y*scale+offset.y,points[1].position.z, 0));
        // computeShader.SetVector("point2" , new Vector4(points[2].position.x*scale+offset.x,points[2].position.y*scale+offset.y,points[2].position.z, 0));
        // computeShader.SetVector("point3" , new Vector4(points[3].position.x*scale+offset.x,points[3].position.y*scale+offset.y,points[3].position.z, 0));
        
        computeShader.SetFloat("thickness", tolerance);


        List<Vector2> bezierPoints = points.ConvertAll(x =>
        {
            Vector3 position;
            return new Vector2((position = x.position).x*scale+offset.x, position.y*scale+offset.y);
        });

        int sizeOfVect2 = sizeof(float) * 2;

        ComputeBuffer computeBuffer = new ComputeBuffer(bezierPoints.Count, sizeOfVect2);
        computeBuffer.SetData(bezierPoints);
        
        computeShader.SetBuffer(0, "pointsBuffer", computeBuffer);
        
        computeShader.Dispatch(0, renderTexture.width/8, renderTexture.height/8, 1);

        Vector2[] temp = new Vector2[bezierPoints.Count];
        
        // computeBuffer.GetData(temp);
        //
        // foreach (var VARIABLE in temp)
        // {
        //     Debug.Log(VARIABLE.ToString());
        // }
        
        UpdateTextureFromRenderTexture(renderTexture);
        
        computeBuffer.Dispose();
        
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
