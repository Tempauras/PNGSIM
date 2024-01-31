using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MouthFiller : MonoBehaviour
{
    [SerializeField] private List<BezierCurves> _bezierCurvesList;
    
    private SpriteRenderer _spriteRenderer;
    public int size;
    public int sizeTexture;
    public int offset;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Texture2D  texture2D = new Texture2D(sizeTexture, sizeTexture);

        Sprite sprite = Sprite.Create(texture2D, new Rect(0,0, sizeTexture,sizeTexture), new Vector2(0.5f,0.5f), sizeTexture);

        _spriteRenderer.sprite = sprite;

    }
    
    
    private void UpdateGraph()
    {
        //DrawBezier(_spriteRenderer, _bezierCurvesList[0]);
        DrawBezier(_bezierCurvesList[0]);
    }

    private void DrawBezier(BezierCurves bezierCurves)
    {
        
        for (int i = 0; i < _spriteRenderer.sprite.texture.width; i++)
        {
            for (int j = 0; j < _spriteRenderer.sprite.texture.height; j++)
            {
                _spriteRenderer.sprite.texture.SetPixel(i, j, Color.white);
            }
        }
        
        List<Vector2> Points = CubeBezier(
            bezierCurves.Points[0].position,
            bezierCurves.Points[1].position,
            bezierCurves.Points[2].position,
            bezierCurves.Points[3].position,
            _spriteRenderer.sprite.texture.width
            );
        
        foreach (var coord in Points)
        {
            _spriteRenderer.sprite.texture.SetPixel((int)(coord.x * size) + offset, (int)(coord.y * size) + offset, Color.black);
        }
        _spriteRenderer.sprite.texture.Apply();
        
    }

    List<Vector2> CubeBezier(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3, int sampleCount)
    {
        List<Vector2> points = new List<Vector2>();
        
        float t = 0f;
        for (int i = 0; i < sampleCount; i++)
        {
            //Cubic formula for bezier curves
            Vector2 B = BezierCurves.Cb(1-t) * point0 + 3 * BezierCurves.Sqr(1-t) *t *point1 + 3 * (1-t) * BezierCurves.Sqr(t) * point2 + BezierCurves.Cb(t) * point3;
            
            points.Add(B);
            t += (1 / (float)sampleCount);
        }

        return points;
    }
}
