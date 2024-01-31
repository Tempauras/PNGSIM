using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Traits", fileName = "New Trait")]
public class Trait : ScriptableObject
{
    [Header("Global traits")]
    public string TraitName;
    public string TraitDescription;

    [Header("Effect on Emotion")]
    public List<EmotionValue> TraitsEffect = new List<EmotionValue>(Enumerable.Range(0, 6).Select(i => new EmotionValue(i)));

    [Header("Non compatible trait")]
    public List<Trait> NonCompatibleTrait = new List<Trait>();
}
