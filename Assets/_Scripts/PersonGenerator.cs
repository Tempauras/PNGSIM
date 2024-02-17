using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class PersonGenerator : MonoBehaviour
{
    [SerializeField] private List<Trait> _traitListToGenerateFrom = new List<Trait>();
    [SerializeField, Tooltip("x is min inclusive, y is max exclusive")] private Vector2 _numberOfTraitsRange;
    [SerializeField] private List<string> _nameListToGenerateFrom = new List<string>();
    [SerializeField, Tooltip("x is min inclusive, y is max exclusive")] private Vector2 _ageRange;
    [SerializeField] private FaceController _faceController;
    
    private int _numberOfPersonToGenerate = 6;

    public List<Person> Persons = new List<Person>();

    // Start is called before the first frame update


    public List<Trait> TraitListToGenerateFrom => _traitListToGenerateFrom;

    public FaceController FaceController => _faceController;


    void Start()
    {
        GeneratePersonOnClick();
    }

    public void ModifyNumberOfPersonsToGenerate(TMP_InputField tmp)
    {
        if (tmp.text == string.Empty)
        {
            _numberOfPersonToGenerate = 0;
        }
        else
        {
            _numberOfPersonToGenerate = int.Parse(tmp.text);
        }
        
    }

    public void GeneratePersonOnClick()
    {
        Persons.Clear();
        for (int i = 0; i < _numberOfPersonToGenerate; i++)
        {
            Person person = GeneratePerson();
            Persons.Add(person);

            person.Description = PhraseGenerator.instance.GeneratePhrase(person);
            
            Debug.Log($"Generated new Person number {i + 1}, Name: {Persons[i].Name}, Age: {Persons[i].Age} with {Persons[i].Traits.Count} traits");
        }
    }

    private Person GeneratePerson()
    {
        int age = Random.Range((int)_ageRange.x, (int)_ageRange.y);
        int numberOfTraits = Random.Range((int) _numberOfTraitsRange.x, (int) _numberOfTraitsRange.y);
        List<Trait> personTraits = new List<Trait>();
        for (int i = 0; i < numberOfTraits; i++)
        {
            PickTrait(ref personTraits);
        }
        string name = _nameListToGenerateFrom[Random.Range(0, _nameListToGenerateFrom.Count)];

        Person person = new Person(name, age, personTraits);
        return person;
    }

    //PROCEDURAL AF :D SO METAL ! 
    private void PickTrait(ref List<Trait> CurrentTraits)
    {
        bool hasSelectedNewCorrectTraits = false;

        // while(!hasSelectedNewCorrectTraits)
        // {
        for (int i = 0; i < _traitListToGenerateFrom.Count && !hasSelectedNewCorrectTraits; i++)
        {
            Trait newTrait = _traitListToGenerateFrom[Random.Range(0, _traitListToGenerateFrom.Count)];
            if (!CurrentTraits.Find(x => x == newTrait) && !CurrentTraits.Exists(x => x.NonCompatibleTrait.Exists(x => x == newTrait)))
            {
                Debug.Log("Adding " + newTrait.name + " to current traits.");
                CurrentTraits.Add(newTrait);
                hasSelectedNewCorrectTraits = true;
            }
        }
    }

    public void FeedFaceController(Person person)
    {
        List<float> highestBlend = new List<float>();
        List<float> secondHighestBlend = new List<float>();
        for (int i = 0; i < 10; i++)
        {
            highestBlend.Add(0.0f);
            secondHighestBlend.Add(0.0f);
        }
        int highestIndex = person.AverageEVs.IndexOf(
            person.AverageEVs.
            Distinct().
            OrderByDescending(ev => ev.Value).
            First());

        int secondHighestIndex = person.AverageEVs.IndexOf(
            person.AverageEVs.
            Distinct().
            OrderByDescending(ev => ev.Value).
            Skip(1).
            First());

        Debug.Log($"Highest blend is {person.AverageEVs[highestIndex].Emotion} with a value of {person.AverageEVs[highestIndex].Value} for {person.Name}");
        Debug.Log($"Second highest blend is {person.AverageEVs[secondHighestIndex].Emotion} with a value of {person.AverageEVs[secondHighestIndex].Value} for {person.Name}");

        switch (person.AverageEVs[highestIndex].Emotion)
        {
            case Emotion.Happiness:
                highestBlend = _faceController.HappinessBlend();
                break;
            case Emotion.Sadness:
                highestBlend = _faceController.SadnessBend();
                break;
            case Emotion.Fear:
                highestBlend = _faceController.FearBlend();
                break;
            case Emotion.Disgust:
                highestBlend = _faceController.DisgustBlend();
                break;
            case Emotion.Anger:
                highestBlend = _faceController.AngerBlend();
                break;
            case Emotion.Surprise:
                highestBlend = _faceController.SurpriseBlend();
                break;
            default:
                break;
        }
        switch (person.AverageEVs[secondHighestIndex].Emotion)
        {
            case Emotion.Happiness:
                secondHighestBlend = _faceController.HappinessBlend();
                break;
            case Emotion.Sadness:
                secondHighestBlend = _faceController.SadnessBend();
                break;
            case Emotion.Fear:
                secondHighestBlend = _faceController.FearBlend();
                break;
            case Emotion.Disgust:
                secondHighestBlend = _faceController.DisgustBlend();
                break;
            case Emotion.Anger:
                secondHighestBlend = _faceController.AngerBlend();
                break;
            case Emotion.Surprise:
                secondHighestBlend = _faceController.SurpriseBlend();
                break;
            default:
                break;
        }

        //We divide by 2 to assure that the face will always look closer to the highest blend
        float alphaForLerp = person.AverageEVs[secondHighestIndex].Value / 2;

        _faceController.MouthOpen = Mathf.Lerp(highestBlend[0], secondHighestBlend[0], alphaForLerp);
        _faceController.UpperLip = Mathf.Lerp(highestBlend[1], secondHighestBlend[1], alphaForLerp);
        _faceController.LowerLip = Mathf.Lerp(highestBlend[2], secondHighestBlend[2], alphaForLerp);
        _faceController.Rounded = Mathf.Lerp(highestBlend[3], secondHighestBlend[3], alphaForLerp);
        _faceController.Smiling = Mathf.Lerp(highestBlend[4], secondHighestBlend[4], alphaForLerp);
        _faceController.LeftRotation = Mathf.Lerp(highestBlend[5], secondHighestBlend[5], alphaForLerp);
        _faceController.RightRotation = Mathf.Lerp(highestBlend[6], secondHighestBlend[6], alphaForLerp);
        _faceController.LeftElevation = Mathf.Lerp(highestBlend[7], secondHighestBlend[7], alphaForLerp);
        _faceController.RightElevation = Mathf.Lerp(highestBlend[8], secondHighestBlend[8], alphaForLerp);
        _faceController.Lids = Mathf.Lerp(highestBlend[9], secondHighestBlend[9], alphaForLerp);

        _faceController.UpdateColours(person.Colour);
    }

    private void AddBlendsTogether(ref List<float>  OriginalBlend, List<float> BlendToAdd)
    {
        if (OriginalBlend.Count == BlendToAdd.Count)
        {
            for (int i = 0; i < OriginalBlend.Count; i++)
            {
                OriginalBlend[i] += BlendToAdd[i];
            }
        }
    }
}
