using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Person
{
    private string _name;
    private int _age;
    private List<Trait> _traits = new List<Trait>();
    private Color _colour;
    private List<EmotionValue> _averageEVs = new List<EmotionValue>();
    private string _description;

    public string Name { get => _name; }
    public int Age { get => _age; }
    public List<Trait> Traits { get => _traits; }
    public Color Colour { get => _colour; }
    public List<EmotionValue> AverageEVs { get => _averageEVs; }

    public string Description
    {
        get => _description;
        set => _description = value;
    }


    public Person(string Name, int Age, List<Trait> Traits)
    {
        _name = Name;
        _age = Age;
        _traits = Traits;
        CalculateParameters();
    }

    public void AddTrait(Trait trait)
    {
        _traits.Add(trait);
        CalculateParameters();
        _description = PhraseGenerator.instance.GeneratePhrase(this);
    }

    public void RemoveTrait(Trait trait)
    {
        _traits.Remove(trait);
        CalculateParameters();
        _description = PhraseGenerator.instance.GeneratePhrase(this);
    }

    private void CalculateParameters()
    {
        _averageEVs.Clear();
        
        List<float> Emotions = new List<float>();
        Emotions.Add(0.0f); //Happiness
        Emotions.Add(0.0f); //Sadness
        Emotions.Add(0.0f); //Fear
        Emotions.Add(0.0f); //Disgust
        Emotions.Add(0.0f); //Anger
        float Surprise = 0.0f;
        foreach (Trait trait in _traits)
        {
            Emotions[0] += trait.TraitsEffect[0].Value;
            Emotions[1] += trait.TraitsEffect[1].Value;
            Emotions[2] += trait.TraitsEffect[2].Value;
            Emotions[3] += trait.TraitsEffect[3].Value;
            Emotions[4] += trait.TraitsEffect[4].Value;
            Surprise += trait.TraitsEffect[5].Value;
        }
        switch (Emotions.IndexOf(Emotions.Max()))
        {
            case 0:
                _colour = Color.yellow;
                break;
            case 1:
                _colour = Color.blue;
                break;
            case 2:
                _colour = new Color(0.3f, 0.0f, 0.3f);
                break;
            case 3:
                _colour = Color.green;
                break;
            case 4:
                _colour = Color.red;
                break;
            default:
                break;
        }
        if (Surprise != 0)
        {
            _colour *= (Surprise / Traits.Count / PNGSIM_Globals.EMOTION_MAXIMUM_RATING);
            _colour.a = 1;
        }


        for (int i = 0; i < Emotions.Count; i++)
        {
            _averageEVs.Add(new EmotionValue(i, Emotions[i] / _traits.Count / PNGSIM_Globals.EMOTION_MAXIMUM_RATING));
        }
        _averageEVs.Add(new EmotionValue(5, Surprise / Traits.Count / PNGSIM_Globals.EMOTION_MAXIMUM_RATING));
    }
    
    public List<Trait> GetAllTraitIncompatibleWithCurrentTraits()
    {
        List<Trait> incompTraits = new List<Trait>();
        foreach (Trait trait in _traits)
        {
            incompTraits.AddRange(trait.NonCompatibleTrait);
        }

        return incompTraits;
    }
}
