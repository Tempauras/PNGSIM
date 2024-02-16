using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FaceController : MonoBehaviour
{
    [SerializeField] private Shader _shader;
    
    [Header("Mouth")]
    
    [FormerlySerializedAs("points")][SerializeField] private List<Transform> pointsTransforms;
    
    [Range(0, 1), SerializeField] private float mouthOpen;
    [Range(0, 1), SerializeField] private float upperLip;
    [Range(0, 1), SerializeField] private float lowerLip;
    [Range(0, 1), SerializeField] private float rounded;
    [Range(0, 1), SerializeField] private float smiling;

    public UnityEvent refreshBeziers;
    public float MouthOpen
    {
        get => mouthOpen;
        set
        {
            mouthOpen = Mathf.Clamp(value, 0.0f, 1.0f);
        
            UpdateMouthOpen();
            refreshBeziers.Invoke();
        }
    }

    public float UpperLip
    {
        get => upperLip;
        set
        {
            upperLip = Mathf.Clamp(value, 0.0f, 1.0f);
            
            UpdateUpperLip();
            refreshBeziers.Invoke();
        }
    }

    public float LowerLip
    {
        get => lowerLip;
        set
        {
            lowerLip = Mathf.Clamp(value, 0.0f, 1.0f); 
            
            UpdateLowerLip();
            refreshBeziers.Invoke();
        }
    }

    public float Rounded
    {
        get => rounded;
        set
        {
            rounded = Mathf.Clamp(value, 0.0f, 1.0f);
            
            UpdateMouthShape();
            refreshBeziers.Invoke();
        }
    }

    public float Smiling
    {
        get => smiling;
        set
        {
            smiling = Mathf.Clamp(value, 0.0f, 1.0f); 
            
            UpdateSmile();
            refreshBeziers.Invoke();
        }
    }



    [Header("Brow")] 
    
    [FormerlySerializedAs("brows")] [SerializeField] private List<Transform> browsTransforms;
    
    [FormerlySerializedAs("LeftRotation")] [Range(0, 1), SerializeField] private float leftRotation = 0.5f;
    [FormerlySerializedAs("RightRotation")] [Range(0, 1), SerializeField] private float rightRotation = 0.5f;
    [FormerlySerializedAs("LeftElevation")] [Range(0, 1), SerializeField] private float leftElevation = 0.25f;
    [FormerlySerializedAs("RightElevation")] [Range(0, 1), SerializeField] private float rightElevation = 0.25f;
    
    
    public float LeftRotation
    {
        get => leftRotation;
        set
        {
            leftRotation = Mathf.Clamp(value, 0.0f, 1.0f); 
        
            UpdateBrowRotation();
        }
    }

    public float RightRotation
    {
        get => rightRotation;
        set
        {
            rightRotation = Mathf.Clamp(value, 0.0f, 1.0f);
        
            UpdateBrowRotation();
        }
    }

    public float LeftElevation
    {
        get => leftElevation;
        set
        {
            leftElevation = Mathf.Clamp(value, 0.0f, 1.0f); 
        
            UpdateBrowPosition();
        }
    }

    public float RightElevation
    {
        get => rightElevation;
        set
        {
            rightElevation = Mathf.Clamp(value, 0.0f, 1.0f);
        
            UpdateBrowPosition();
        }
    }
    
    
    [Header("Lids")]
    
    [SerializeField] private Transform lidsTransform;

    [Range(0, 1), SerializeField] private float lids;


    public float Lids
    {
        get => lids;
        set
        {
            lids = Mathf.Clamp(value, 0.0f, 1.0f);
            
            UpdateLidPosition();
        }
    }


    [SerializeField] private bool batEyeLid = true;
    [EnableIf("batEyeLid"), SerializeField] private Vector2 timeBetweenBlink = new (2f,10f);
    [EnableIf("batEyeLid"), SerializeField] private Vector2 timeForBlink = new (100f,400f);

    [Header("Pupils")] 
    
    [SerializeField] private List<Transform> pupils;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float pupilMoveDistance = 0.1f;

    private float _localScale;
    
    // mouth Values
    private float _maxYPosRoot;
    private float _minYPosRoot;

    private const float MaxYLipOffset = .3f;

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

    private const float MaxLidYScale = 0f;
    private const float MinLidYScale = 1.1f;

    public UnityEvent initFinished;

    private void Start()
    {
        _originalLowerLipPosYLeft = pointsTransforms[6].transform.localPosition.y;
        _originalLowerLipPosYRight = pointsTransforms[7].transform.localPosition.y;
        _originalUpperLipPosYLeft = pointsTransforms[3].transform.localPosition.y;
        _originalUpperLipPosYRight = pointsTransforms[4].transform.localPosition.y;
        
        _maxYPosRoot = pointsTransforms[2].transform.localPosition.y;
        
        _localScale = transform.localScale.x;
        
        StartCoroutine(EyeLids());
        
        OnValidate();
        
        
        initFinished.Invoke();
    }

    private void OnValidate()
    {
        //Rounded
        UpdateMouthShape();
        
        //Upper Lip
        UpdateUpperLip();
        
        //Lower lip
        UpdateLowerLip();
        
        //Smiling
        UpdateSmile();
        
        //OpenMouth
        UpdateMouthOpen();
        
        //BrowRotation
        UpdateBrowRotation();
        
        //BrowPosition
        UpdateBrowPosition();
        
        //LidPosition
        UpdateLidPosition();
        
        refreshBeziers.Invoke();
    }

    private void Update()
    {
        // //Rounded
        // UpdateMouthShape();
        //
        // //Upper Lip
        // UpdateUpperLip();
        //
        // //Lower lip
        // UpdateLowerLip();
        //
        // //Smiling
        // UpdateSmile();
        //
        // //OpenMouth
        // UpdateMouthOpen();
        //
        // //BrowRotation
        // UpdateBrowRotation();
        //
        // //BrowPosition
        // UpdateBrowPosition();
        //
        // //LidPosition
        // UpdateLidPosition();
        
        
        pupils[0].transform.localPosition = Vector2.zero;
        pupils[1].transform.localPosition = Vector2.zero;
        
        Vector2 directionOfMousePupilLeft =(Input.mousePosition - mainCamera.WorldToScreenPoint(pupils[0].transform.position)).normalized;
        Vector2 directionOfMousePupilRight =(Input.mousePosition - mainCamera.WorldToScreenPoint(pupils[1].transform.position)).normalized;

        pupils[0].transform.localPosition = directionOfMousePupilLeft * pupilMoveDistance;
        pupils[1].transform.localPosition = directionOfMousePupilRight * pupilMoveDistance;
    }

    private void UpdateMinRootPos()
    {
        _minYPosRoot = pointsTransforms[5].transform.localPosition.y;
    }
    
    private void UpdateLidPosition()
    {
        float leftScaleLid = Mathf.Lerp(MaxLidYScale, MinLidYScale, lids);
        
        lidsTransform.transform.localScale = new Vector3(1.1f, leftScaleLid, 1);
    }

    private void UpdateMouthShape()
    {
        float spread = Mathf.Lerp(MaxSeparation, MinSeparation, rounded) * _localScale;

        float oneSideValue = spread / 2;
        
        pointsTransforms[3].transform.position = new Vector3(-oneSideValue,pointsTransforms[3].transform.position.y,pointsTransforms[3].transform.position.z);
        pointsTransforms[4].transform.position = new Vector3(oneSideValue,pointsTransforms[4].transform.position.y,pointsTransforms[4].transform.position.z);
        pointsTransforms[6].transform.position = new Vector3(-oneSideValue,pointsTransforms[6].transform.position.y,pointsTransforms[6].transform.position.z);
        pointsTransforms[7].transform.position = new Vector3(oneSideValue,pointsTransforms[7].transform.position.y,pointsTransforms[7].transform.position.z);
    }

    private void UpdateUpperLip()
    {
        float openUpper = Mathf.Lerp(0,MaxYLipOffset, upperLip);
        
        pointsTransforms[3].transform.localPosition = new Vector3(pointsTransforms[3].transform.localPosition.x,_originalUpperLipPosYLeft - openUpper,pointsTransforms[3].transform.localPosition.z);
        pointsTransforms[4].transform.localPosition = new Vector3(pointsTransforms[4].transform.localPosition.x,_originalUpperLipPosYRight - openUpper,pointsTransforms[4].transform.localPosition.z);
    }

    private void UpdateLowerLip()
    {
        float openLower = Mathf.Lerp(0,MaxYLipOffset, lowerLip);
        
        pointsTransforms[6].transform.localPosition = new Vector3(pointsTransforms[6].transform.localPosition.x, _originalLowerLipPosYLeft + openLower, pointsTransforms[6].transform.localPosition.z);
        pointsTransforms[7].transform.localPosition = new Vector3(pointsTransforms[7].transform.localPosition.x, _originalLowerLipPosYRight + openLower, pointsTransforms[7].transform.localPosition.z);
    }

    private void UpdateSmile()
    {
        float yPos = Mathf.Lerp(_maxYPosRoot, _minYPosRoot, smiling) * _localScale;
        pointsTransforms[0].transform.position = new Vector3(pointsTransforms[0].transform.position.x,yPos ,pointsTransforms[0].transform.position.z);
        pointsTransforms[1].transform.position = new Vector3(pointsTransforms[1].transform.position.x,yPos ,pointsTransforms[1].transform.position.z);
    }

    private void UpdateMouthOpen()
    {
        float openValue = Mathf.Lerp(MaxLowerJawPos, MinLowerJawPos, mouthOpen); 
        pointsTransforms[5].transform.localPosition = new Vector3(pointsTransforms[5].transform.localPosition.x,openValue,pointsTransforms[5].transform.localPosition.z);
        UpdateMinRootPos();
    }
    
    private void UpdateBrowRotation()
    {
        float rotateValueLeft = Mathf.Lerp(MaxBrowRotateValue,-MaxBrowRotateValue,leftRotation);
        float rotateValueRight = Mathf.Lerp(-MaxBrowRotateValue,MaxBrowRotateValue,rightRotation);

        browsTransforms[0].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateValueLeft));
        browsTransforms[1].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateValueRight));
    }

    private void UpdateBrowPosition()
    {
        float elevateValueLeft = Mathf.Lerp(0,MaxBrowHeightValue,leftElevation);
        float elevateValueRight = Mathf.Lerp(0,MaxBrowHeightValue,rightElevation);
        
        browsTransforms[0].transform.localPosition = new Vector3(-0.15f, elevateValueLeft, 0);
        browsTransforms[1].transform.localPosition = new Vector3(0.15f, elevateValueRight, 0);
    }

    public void UpdateColours(Color colour)
    {
        _shader.UpdateColours(colour);
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

    public List<float> HappinessBlend()
    {
        List<float> blend = new List<float>();
        blend.Add(0.01f); //mouthOpen
        blend.Add(0.2f); //upperLip
        blend.Add(0.01f); //lowerLip
        blend.Add(0.4f); //rounded
        blend.Add(0.01f); //smiling
        blend.Add(0.5f); //leftRotation
        blend.Add(0.5f); //rightRotation
        blend.Add(0.25f); //leftElevation
        blend.Add(0.25f); //rightElevation
        blend.Add(0.2f); //height

        return blend;
    }

    public List<float> SadnessBend()
    {
        List<float> blend = new List<float>();
        blend.Add(0.01f); //mouthOpen
        blend.Add(0.01f); //upperLip
        blend.Add(0.5f); //lowerLip
        blend.Add(0.4f); //rounded
        blend.Add(1f); //smiling
        blend.Add(0.01f); //leftRotation
        blend.Add(0.01f); //rightRotation
        blend.Add(0.01f); //leftElevation
        blend.Add(0.01f); //rightElevation
        blend.Add(0.2f); //height

        return blend;
    }

    public List<float> FearBlend()
    {
        List<float> blend = new List<float>();
        blend.Add(0.35f); //mouthOpen
        blend.Add(0.01f); //upperLip
        blend.Add(0.1f); //lowerLip
        blend.Add(0.5f); //rounded
        blend.Add(0.9f); //smiling
        blend.Add(0.18f); //leftRotation
        blend.Add(0.1f); //rightRotation
        blend.Add(0.5f); //leftElevation
        blend.Add(0.9f); //rightElevation
        blend.Add(0.01f); //height

        return blend;
    }

    public List<float> DisgustBlend()
    {
        List<float> blend = new List<float>();
        blend.Add(0.15f); //mouthOpen
        blend.Add(0.01f); //upperLip
        blend.Add(0.1f); //lowerLip
        blend.Add(0.7f); //rounded
        blend.Add(0.6f); //smiling
        blend.Add(0.4f); //leftRotation
        blend.Add(0.4f); //rightRotation
        blend.Add(0.75f); //leftElevation
        blend.Add(0.5f); //rightElevation
        blend.Add(0.5f); //height

        return blend;
    }

    public List<float> AngerBlend()
    {
        List<float> blend = new List<float>();
        blend.Add(0.25f); //mouthOpen
        blend.Add(0.01f); //upperLip
        blend.Add(0.6f); //lowerLip
        blend.Add(0.01f); //rounded
        blend.Add(0.9f); //smiling
        blend.Add(1f); //leftRotation
        blend.Add(1f); //rightRotation
        blend.Add(0.01f); //leftElevation
        blend.Add(0.01f); //rightElevation
        blend.Add(0.01f); //height

        return blend;
    }

    public List<float> SurpriseBlend()
    {
        List<float> blend = new List<float>();
        blend.Add(0.5f); //mouthOpen
        blend.Add(0.01f); //upperLip
        blend.Add(0.01f); //lowerLip
        blend.Add(0.1f); //rounded
        blend.Add(0.5f); //smiling
        blend.Add(0.3f); //leftRotation
        blend.Add(0.3f); //rightRotation
        blend.Add(0.5f); //leftElevation
        blend.Add(0.5f); //rightElevation
        blend.Add(0.01f); //height

        return blend;
    }
}
