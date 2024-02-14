using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;


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
            Persons.Add(GeneratePerson());
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

        while(!hasSelectedNewCorrectTraits)
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
        List<float> Blends = new List<float>();
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        Blends.Add(0.0f);
        foreach (EmotionValue ev in person.AverageEVs)
        {
            switch (ev.Emotion)
            {
                case Emotion.Happiness:
                    AddBlendsTogether(ref Blends, _faceController.HappinessBlend(ev.Value));
                    break;
                case Emotion.Sadness:
                    AddBlendsTogether(ref Blends, _faceController.SadnessBend(ev.Value));
                    break;
                case Emotion.Fear:
                    AddBlendsTogether(ref Blends, _faceController.FearBlend(ev.Value));
                    break;
                case Emotion.Disgust:
                    AddBlendsTogether(ref Blends, _faceController.DisgustBlend(ev.Value));
                    break;
                case Emotion.Anger:
                    AddBlendsTogether(ref Blends, _faceController.AngerBlend(ev.Value));
                    break;
                case Emotion.Surprise:
                    AddBlendsTogether(ref Blends, _faceController.SurpriseBlend(ev.Value));
                    break;
                default:
                    break;
            }
        }

        _faceController.MouthOpen = Blends[0];
        _faceController.UpperLip = Blends[1];
        _faceController.LowerLip = Blends[2];
        _faceController.Rounded = Blends[3];
        _faceController.Smiling = Blends[4];
        _faceController.LeftRotation = Blends[5];
        _faceController.RightRotation = Blends[6];
        _faceController.LeftElevation = Blends[7];
        _faceController.RightElevation = Blends[8];
        _faceController.Lids = Blends[9];
    }

    private void AddBlendsTogether(ref List<float>  OriginalBlend, List<float> BlendToAdd)
    {
        if (OriginalBlend.Count == BlendToAdd.Count)
        {
            for (int i = 0; i < OriginalBlend.Count; i++)
            {
                OriginalBlend[i] =+ BlendToAdd[i];
            }
        }
    }
}
