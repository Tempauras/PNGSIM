using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class BezierCurves : MonoBehaviour
{
    [SerializeField] private List<Transform> points = new ();
    [SerializeField] private int numberOfLines = 200;

    [SerializeField] private bool isCubeFormula = true;
    
    private LineRenderer _lineRenderer;

    public List<Transform> Points => points;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.alignment = LineAlignment.View;
        _lineRenderer.positionCount = numberOfLines;
    }

    void Update()
    {
        if (isCubeFormula)
        {
            DrawCubeBezierCurve(points[0].position, points[1].position, points[2].position, points[3].position);
            return;
        }
        DrawQuadraticBezierCurve(points[0].position, points[1].position, points[2].position);
    }

    void DrawCubeBezierCurve(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3)
    {
        float t = 0f;
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            //Cubic formula for bezier curves
            Vector2 B = Cb(1-t) * point0 + 3 * Sqr(1-t) *t *point1 + 3 * (1-t) * Sqr(t) * point2 + Cb(t) * point3;
            
            _lineRenderer.SetPosition(i, B);
            t += (1 / (float)_lineRenderer.positionCount);
        }
        _lineRenderer.positionCount = numberOfLines + 1;
        _lineRenderer.SetPosition(numberOfLines, points[3].position);
    }
    
    void DrawQuadraticBezierCurve(Vector2 point0, Vector2 point1, Vector2 point2)
    {
        float t = 0f;
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            //Quadratic formula for bezier curves
            Vector2 B = Sqr(1 - t) * point0 + 2 * (1 - t) * t * point1 + Sqr(t) * point2;
            
            _lineRenderer.SetPosition(i, B);
            t += (1 / (float)_lineRenderer.positionCount);
        }
        _lineRenderer.positionCount = numberOfLines + 1;
        _lineRenderer.SetPosition(numberOfLines, points[2].position);
    }
    
    public static float Sqr(float f)
    {
        return f * f;
    }
    
    public static float Cb(float f)
    {
        return f * f * f;
    }
}