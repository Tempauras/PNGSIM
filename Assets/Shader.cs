using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class Shader : MonoBehaviour
{
    [SerializeField] private FaceController faceController;
    
    [FormerlySerializedAs("mainCam")] [SerializeField] private Camera Cam;
    [SerializeField] private List<Transform> points;
    [SerializeField] private List<SpriteRenderer> lidsSpriteRenderers;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float scale;
    [SerializeField] private float thickness;
    
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int _kernelDraw;
    private int _kernelCalculateBezier;
    private ComputeBuffer _pointBuffer;
    private ComputeBuffer _computeBuffer;

    private void Start()
    {
        UpdateColours(spriteRenderer.color);
        
        //create a render texture for the compute shader to draw on 
        renderTexture = new RenderTexture(Cam.pixelWidth, Cam.pixelHeight, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        //Setup sprite for sprite renderer
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        texture2D.filterMode = FilterMode.Point;
        
        Sprite sprite = Sprite.Create(texture2D, new Rect(0,0, texture2D.width,texture2D.height), new Vector2(0.5f,0.5f), 100);
        spriteRenderer.sprite = sprite;
        
        //Add texture to shader on material
        spriteRenderer.material.SetTexture("_RenderTex", renderTexture);

        //Setting up the compute shader
        
        //getting kernels
        _kernelDraw = computeShader.FindKernel("CSDraw");
        _kernelCalculateBezier = computeShader.FindKernel("CalculateBezierPoints");
        
        //setting variables to draw in compute shader
        computeShader.SetTexture(_kernelDraw,"Result" , renderTexture);
        computeShader.SetInt("size", renderTexture.width);
        computeShader.SetFloat("thickness", thickness);
        
        //setup point buffer to use in compute shader, essentially making a semi-dynamic array
        int sizeOfVect2 = sizeof(float) * 2;
        _computeBuffer = new ComputeBuffer(renderTexture.width*2, sizeOfVect2);
        _pointBuffer = new ComputeBuffer(points.Count, sizeOfVect2);
        UpdatePointBuffer();
        Vector2[] arrayForBezierPoints = new Vector2[renderTexture.width*2];
        _computeBuffer.SetData(arrayForBezierPoints);
        
        //giving that array to both kernels of compute shader
        computeShader.SetBuffer(_kernelDraw, "points_buffer",  _computeBuffer);
        computeShader.SetBuffer(_kernelCalculateBezier, "points_buffer",  _computeBuffer);
        
        faceController.refreshBeziers.AddListener(() => StartCoroutine(UpdateFrame()));
    }

    private IEnumerator UpdateFrame()
    {
        //updates the 4 points of the 2 bezier we want to draw 
        UpdatePointBuffer();
        computeShader.SetBuffer(_kernelCalculateBezier, "points",  _pointBuffer);
            
        //calls the dispatch, essentially tells the compute shaders to run 
                    
        computeShader.Dispatch(_kernelCalculateBezier, 1,1,1);
                    
        computeShader.Dispatch(_kernelDraw, renderTexture.width/10 + 1, renderTexture.height/10 + 1, 1);
        yield return null;
    }

    
    private void UpdatePointBuffer()
    {
        // creates a list of screen space vector2 from the transform points of the bezier  
        List<Vector2> bezierPoints = points.ConvertAll(x =>
        {
            Vector3 position;
            Vector2 pos = new Vector2((position = x.position).x, position.y);
            return (Vector2)Cam.WorldToScreenPoint(new Vector2(pos.x*scale+offset.x, pos.y*scale+offset.y));
        });
        
        //sets the data to the compute buffer
        _pointBuffer.SetData(bezierPoints);
    }

    public void UpdateColours(Color colour)
    {
        spriteRenderer.color = colour;
        
        Cam.backgroundColor = spriteRenderer.color;

        foreach (SpriteRenderer lidsSpriteRenderer in lidsSpriteRenderers)
        {
            lidsSpriteRenderer.color = spriteRenderer.color * Color.grey; 
        }
    }

    private void OnDisable()
    {
        //release everything, no longer needed
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        
        _pointBuffer.Release();
    }

    private void OnDestroy()
    {
        //release everything, no longer needed
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        
        _pointBuffer.Release();
    }
}
