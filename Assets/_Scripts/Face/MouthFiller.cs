using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class MouthFiller : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteMask _spriteMask;
    [SerializeField] private int sizeTexture;
    [SerializeField] private List<Transform> Beziers = new List<Transform>();

    [SerializeField] private Color mouthColor;
    [SerializeField] private Color otherColor;


    private void Update()
    {
	    //if uncoment this prepare for death
	    //UpdateGraph();
    }

    [Button("UpdateMesh")]
    private void UpdateGraph()
    {
        Texture2D texture2D = new Texture2D(sizeTexture, sizeTexture);

        Sprite sprite = Sprite.Create(texture2D, new Rect(0,0, sizeTexture,sizeTexture), new Vector2(0.5f,0.5f), sizeTexture/2, 0, SpriteMeshType.FullRect);

        _spriteRenderer.sprite = sprite;
        
        ResetImage();
        DrawBezier(Beziers.Take(4).ToList());
        DrawBezier(Beziers.Skip(4).Take(4).ToList());
        
        Vector2 origin = FloodFillOrigin();
        
        if (origin != Vector2.zero)
        {

	        Texture2D writetexture2D = new Texture2D(texture2D.width, texture2D.height);
	        writetexture2D.SetPixels(texture2D.GetPixels());
	        writetexture2D.Apply();
	        FloodFill(_spriteRenderer.sprite.texture, writetexture2D, otherColor, 0, (int)origin.x, (int)origin.y);
	        Sprite spriteNew = Sprite.Create(writetexture2D, new Rect(0,0, sizeTexture,sizeTexture), new Vector2(0.5f,0.5f), sizeTexture/2,0, SpriteMeshType.FullRect);
	        _spriteMask.sprite = spriteNew;
        }
    }

    private Vector2 FloodFillOrigin()
    {
	    int blackCounter = 0;
	    bool isNextWhite = false;
	    Vector2 point = Vector2.zero;
	    
	    for (int i = 0; i < sizeTexture; i++)
	    {
		    Color color = _spriteRenderer.sprite.texture.GetPixel(sizeTexture / 2,i );

		    if (color == mouthColor)
		    {
			    blackCounter++;
			    Color colorWhite = _spriteRenderer.sprite.texture.GetPixel(sizeTexture / 2,i+1);
			    if (colorWhite == otherColor && point == Vector2.zero)
			    {
				    isNextWhite = true;
                               
				    point = new Vector2(sizeTexture / 2, i + 1);
			    }
		    }
	    }

	    if (blackCounter >= 2 && isNextWhite)
	    {
		    //Debug.Log($"True : {point.ToString()}");
		    return point;
	    }

	    //Debug.Log("False");
	    return point;
    }

    private void ResetImage()
    {
	    //TODO : this is slow af
	    
	    int width = _spriteRenderer.sprite.texture.width;
	    int height = _spriteRenderer.sprite.texture.height;
	    
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _spriteRenderer.sprite.texture.SetPixel(i, j, otherColor);
            }
        }
    }

    private void DrawBezier(List<Transform> Points)
    {
        List<Vector2> BesierPoints = CubeBezier(
            Points[0].position,
            Points[1].position,
            Points[2].position,
            Points[3].position,
            _spriteRenderer.sprite.texture.width
            );
        
        foreach (var coord in BesierPoints)
        {
            _spriteRenderer.sprite.texture.SetPixel((int)(coord.x * sizeTexture/2) + sizeTexture/2 , (int)(coord.y * sizeTexture/2) + sizeTexture/2, mouthColor);
            _spriteRenderer.sprite.texture.SetPixel((int)(coord.x * sizeTexture/2) + sizeTexture/2  - 1, (int)(coord.y * sizeTexture/2) + sizeTexture/2, mouthColor);
            _spriteRenderer.sprite.texture.SetPixel((int)(coord.x * sizeTexture/2) + sizeTexture/2 , (int)(coord.y * sizeTexture/2) + sizeTexture/2 - 1, mouthColor);
            _spriteRenderer.sprite.texture.SetPixel((int)(coord.x * sizeTexture/2) + sizeTexture/2  + 1, (int)(coord.y * sizeTexture/2) + sizeTexture/2, mouthColor);
            _spriteRenderer.sprite.texture.SetPixel((int)(coord.x * sizeTexture/2) + sizeTexture/2  , (int)(coord.y * sizeTexture/2) + sizeTexture/2 + 1, mouthColor);
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
    
    
	public struct Point {

		public int x;
		public int y;

		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	public void FloodFill(Texture2D readTexture, Texture2D writeTexture, Color sourceColor, float tollerance, int x, int y) {
		var targetColor = mouthColor;
		var q = new Queue<Point> (readTexture.width * readTexture.height);
		q.Enqueue (new Point (x, y));
		int iterations = 0;

		var width = readTexture.width;
		var height = readTexture.height;
		while (q.Count > 0) {
			var point = q.Dequeue ();
			var x1 = point.x;
			var y1 = point.y;
			if (q.Count > width * height) {
				throw new System.Exception ("The algorithm is probably looping. Queue size: " + q.Count);
			}

			if (writeTexture.GetPixel (x1, y1) == targetColor) {
				continue;
			}

			writeTexture.SetPixel (x1, y1, targetColor);


			var newPoint = new Point (x1 + 1, y1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			newPoint = new Point (x1 - 1, y1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			newPoint = new Point (x1, y1 + 1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			newPoint = new Point (x1, y1 - 1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			iterations++;
		}
		
		writeTexture.Apply();
	}

	static bool CheckValidity(Texture2D texture, int width, int height, Point p, Color sourceColor, float tollerance) {
		if (p.x < 0 || p.x >= width) {
			return false;
		}
		if (p.y < 0 || p.y >= height) {
			return false;
		}

		var color = texture.GetPixel(p.x, p.y);

		var distance = Mathf.Abs (color.r - sourceColor.r) +  Mathf.Abs (color.g - sourceColor.g) +  Mathf.Abs (color.b - sourceColor.b);
		return distance <= tollerance;
	}
    
}
