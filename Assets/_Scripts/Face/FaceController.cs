using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FaceController : MonoBehaviour
{
    [Header("Mouth")]
    
    [FormerlySerializedAs("points")][SerializeField] private List<Transform> pointsTransforms;
    
    [Range(0, 1), SerializeField] private float mouthOpen;
    [Range(0, 1), SerializeField] private float upperLip;
    [Range(0, 1), SerializeField] private float lowerLip;
    [Range(0, 1), SerializeField] private float rounded;
    [Range(0, 1), SerializeField] private float smiling;

    
    [Header("Brow")] 
    
    [FormerlySerializedAs("brows")] [SerializeField] private List<Transform> browsTransforms;
    
    [FormerlySerializedAs("LeftRotation")] [Range(0, 1), SerializeField] private float leftRotation = 0.5f;
    [FormerlySerializedAs("RightRotation")] [Range(0, 1), SerializeField] private float rightRotation = 0.5f;
    [FormerlySerializedAs("LeftElevation")] [Range(0, 1), SerializeField] private float leftElevation = 0.25f;
    [FormerlySerializedAs("RightElevation")] [Range(0, 1), SerializeField] private float rightElevation = 0.25f;
    
    
    [Header("Lids")]
    
    [SerializeField] private Transform lidsTransform;
    
    [SerializeField] private bool batEyeLid = true;
    [EnableIf("batEyeLid"), SerializeField] private Vector2 timeBetweenBlink = new (2f,10f);
    [EnableIf("batEyeLid"), SerializeField] private Vector2 timeForBlink = new (100f,400f);

    [Header("Pupils")] 
    
    [SerializeField] private List<Transform> pupils;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float pupilMoveDistance = 0.1f;
    
    // mouth Values
    private float _maxYPosRoot;
    private float _minYPosRoot;

    private const float MaxYLipOffset = .5f;

    private const float MaxSeparation = 1f;
    private const float MinSeparation = 0f;

    private float _originalLowerLipPosYLeft;
    private float _originalLowerLipPosYRight;
    private float _originalUpperLipPosYLeft;
    private float _originalUpperLipPosYRight;

    private const float MaxLowerJawPos = -0.405f;
    private const float MinLowerJawPos = -0.844f;


    // brow Values
    private const float MaxBrowRotateValue = 40f;
    private const float MaxBrowHeightValue = 0.16f;

    //Lid Values

    private const float MaxLidYScale = 0.2f;
    private const float MinLidYScale = 1.1f;

    private void Start()
    {
        _originalLowerLipPosYLeft = pointsTransforms[6].transform.localPosition.y;
        _originalLowerLipPosYRight = pointsTransforms[7].transform.localPosition.y;
        _originalUpperLipPosYLeft = pointsTransforms[6].transform.localPosition.y;
        _originalUpperLipPosYRight = pointsTransforms[7].transform.localPosition.y;
        
        StartCoroutine(EyeLids());
    }
    
    
    private void Update()
    {
        _maxYPosRoot = pointsTransforms[2].transform.localPosition.y;
        _minYPosRoot = pointsTransforms[5].transform.localPosition.y;
        
        var localScale = transform.localScale.x;
        
        //Rounded
        float spread = Mathf.Lerp(MaxSeparation, MinSeparation, rounded) * localScale;

        float oneSideValue = spread / 2;
        
        pointsTransforms[3].transform.position = new Vector3(-oneSideValue,pointsTransforms[3].transform.position.y,pointsTransforms[3].transform.position.z);
        pointsTransforms[4].transform.position = new Vector3(oneSideValue,pointsTransforms[4].transform.position.y,pointsTransforms[4].transform.position.z);
        pointsTransforms[6].transform.position = new Vector3(-oneSideValue,pointsTransforms[6].transform.position.y,pointsTransforms[6].transform.position.z);
        pointsTransforms[7].transform.position = new Vector3(oneSideValue,pointsTransforms[7].transform.position.y,pointsTransforms[7].transform.position.z);
        
        //Upper Lip

        float openUpper = Mathf.Lerp(0,MaxYLipOffset, upperLip);
        
        pointsTransforms[3].transform.localPosition = new Vector3(pointsTransforms[3].transform.localPosition.x,_originalUpperLipPosYLeft - openUpper,pointsTransforms[3].transform.localPosition.z);
        pointsTransforms[4].transform.localPosition = new Vector3(pointsTransforms[4].transform.localPosition.x,_originalUpperLipPosYRight - openUpper,pointsTransforms[4].transform.localPosition.z);
        
        //Lower lip
        
        float openLower = Mathf.Lerp(0,MaxYLipOffset, lowerLip);
        
        pointsTransforms[6].transform.localPosition = new Vector3(pointsTransforms[6].transform.localPosition.x, _originalLowerLipPosYLeft + openLower, pointsTransforms[6].transform.localPosition.z);
        pointsTransforms[7].transform.localPosition = new Vector3(pointsTransforms[7].transform.localPosition.x, _originalLowerLipPosYRight + openLower, pointsTransforms[7].transform.localPosition.z);
        
        //Smiling

        float yPos = Mathf.Lerp(_maxYPosRoot, _minYPosRoot, smiling) * localScale;
        pointsTransforms[0].transform.position = new Vector3(pointsTransforms[0].transform.position.x,yPos ,pointsTransforms[0].transform.position.z);
        pointsTransforms[1].transform.position = new Vector3(pointsTransforms[1].transform.position.x,yPos ,pointsTransforms[1].transform.position.z);
        
        //OpenMouth
        
        float openValue = Mathf.Lerp(MaxLowerJawPos, MinLowerJawPos, mouthOpen); 
        pointsTransforms[5].transform.localPosition = new Vector3(pointsTransforms[5].transform.localPosition.x,openValue,pointsTransforms[5].transform.localPosition.z);
        
        //BrowRotation
        
        float rotateValueLeft = Mathf.Lerp(MaxBrowRotateValue,-MaxBrowRotateValue,leftRotation);
        float rotateValueRight = Mathf.Lerp(-MaxBrowRotateValue,MaxBrowRotateValue,rightRotation);


        browsTransforms[0].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateValueLeft));
        browsTransforms[1].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateValueRight));
        
        
        //BrowPosition
        
        float elevateValueLeft = Mathf.Lerp(0,MaxBrowHeightValue,leftElevation);
        float elevateValueRight = Mathf.Lerp(0,MaxBrowHeightValue,rightElevation);
        
        browsTransforms[0].transform.localPosition = new Vector3(-0.15f, elevateValueLeft, 0);
        browsTransforms[1].transform.localPosition = new Vector3(0.15f, elevateValueRight, 0);
        
        //LidPosition

        // float leftScaleLid = Mathf.Lerp(MaxLidYScale, MinLidYScale, lids);
        //
        // lidsTransform.transform.localScale = new Vector3(1, leftScaleLid, 1);


        Vector2 directionOfMousePupilLeft =(Input.mousePosition - mainCamera.WorldToScreenPoint(pupils[0].transform.position)).normalized;
        Vector2 directionOfMousePupilRight =(Input.mousePosition - mainCamera.WorldToScreenPoint(pupils[1].transform.position)).normalized;

        pupils[0].transform.localPosition = Vector2.zero + directionOfMousePupilLeft * pupilMoveDistance;
        pupils[1].transform.localPosition = Vector2.zero + directionOfMousePupilRight * pupilMoveDistance;

    }


    private IEnumerator EyeLids()
    {
        float blinkTime = Random.Range(timeForBlink.x, timeForBlink.y)/1000;
        float eyeLidYScale = lidsTransform.transform.localScale.y;

        float time = 0;
        
        while (time <= blinkTime)
        {
            float x = Mathf.InverseLerp(0, blinkTime, time);

            float lerpValue = (Mathf.Cos(x * 2 * Mathf.PI) + 1) / 2;
            
            float yValue = Mathf.Lerp(MinLidYScale,eyeLidYScale, lerpValue);

            lidsTransform.transform.localScale = new Vector3(1.1f,yValue, 1);

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        
        lidsTransform.transform.localScale = new Vector3(1.1f,eyeLidYScale, 1);

        if (batEyeLid)
        { 
            yield return new WaitForSeconds(Random.Range(timeBetweenBlink.x, timeBetweenBlink.y));
            StartCoroutine(EyeLids());
        }
    }
}
