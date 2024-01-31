using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Traits", fileName = "New Trait")]
public class Trait : ScriptableObject
{
    [Header("Global traits")]
    public string TraitName;
    public string TraitDescription;

    [Header("Effect on Emotion")]
    public List<EmotionValue> TraitsEffect = new List<EmotionValue>();

    [Header("Non compatible trait")]
    public List<Trait> NonCompatibleTrait = new List<Trait>();
}
