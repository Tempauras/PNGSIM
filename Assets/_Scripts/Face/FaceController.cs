using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FaceController : MonoBehaviour
{
    [SerializeField] private List<Transform> points;

    [Range(0, 1), SerializeField] private float mouthOpen;
    [Range(0, 1), SerializeField] private float upperLip;
    [Range(0, 1), SerializeField] private float lowerLip;
    [Range(0, 1), SerializeField] private float rounded;
    [Range(0, 1), SerializeField] private float smiling;

    private float _maxYposRoot;
    private float _minYposRoot;
    
    private float maxYLipOffest = .5f;

    private float maxSeparation = 1f;
    private float minSeparation = 0f;

    private float originalLowerLipPosYLeft;
    private float originalLowerLipPosYRight;
    private float originalUpperLipPosYLeft;
    private float originalUpperLipPosYRight;
 
    private float maxLowerJawPos = -0.436296f;
    private float minLowerJawPos = -0.844f;
    private void Start()
    {
        originalLowerLipPosYLeft = points[6].transform.localPosition.y;
        originalLowerLipPosYRight = points[7].transform.localPosition.y;
        originalUpperLipPosYLeft = points[6].transform.localPosition.y;
        originalUpperLipPosYRight = points[7].transform.localPosition.y;
    }

    //TODO : and make lip go 2 times the length of the teeth
    
    private void Update()
    {
        _maxYposRoot = points[2].transform.localPosition.y;
        _minYposRoot = points[5].transform.localPosition.y;
        
        var localScale = transform.localScale.x;
        
        //Rounded
        float spread = Mathf.Lerp(maxSeparation, minSeparation, rounded) * localScale;

        float oneSideValue = spread / 2;
        
        points[3].transform.position = new Vector3(-oneSideValue,points[3].transform.position.y,points[3].transform.position.z);
        points[4].transform.position = new Vector3(oneSideValue,points[4].transform.position.y,points[4].transform.position.z);
        points[6].transform.position = new Vector3(-oneSideValue,points[6].transform.position.y,points[6].transform.position.z);
        points[7].transform.position = new Vector3(oneSideValue,points[7].transform.position.y,points[7].transform.position.z);
        
        //Upper Lip

        float openUpper = Mathf.Lerp(0,maxYLipOffest, upperLip);
        
        points[3].transform.localPosition = new Vector3(points[3].transform.localPosition.x,originalUpperLipPosYLeft - openUpper,points[3].transform.localPosition.z);
        points[4].transform.localPosition = new Vector3(points[4].transform.localPosition.x,originalUpperLipPosYLeft - openUpper,points[4].transform.localPosition.z);
        
        //Lower lip
        
        float openLower = Mathf.Lerp(0,maxYLipOffest, lowerLip);
        
        points[6].transform.localPosition = new Vector3(points[6].transform.localPosition.x, originalLowerLipPosYLeft + openLower, points[6].transform.localPosition.z);
        points[7].transform.localPosition = new Vector3(points[7].transform.localPosition.x, originalLowerLipPosYRight + openLower, points[7].transform.localPosition.z);
        
        //Smiling

        float yPos = Mathf.Lerp(_maxYposRoot, _minYposRoot, smiling) * localScale;
        points[0].transform.position = new Vector3(points[0].transform.position.x,yPos ,points[0].transform.position.z);
        points[1].transform.position = new Vector3(points[1].transform.position.x,yPos ,points[1].transform.position.z);
        
        //OpenMouth
        
        float openValue = Mathf.Lerp(maxLowerJawPos, minLowerJawPos, mouthOpen); 
        points[5].transform.localPosition = new Vector3(points[5].transform.localPosition.x,openValue,points[5].transform.localPosition.z);
    }
}
