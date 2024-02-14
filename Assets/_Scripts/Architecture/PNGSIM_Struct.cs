using System;
using Unity.VisualScripting;

[Serializable]
public struct EmotionValue
{
    public Emotion Emotion;
    public float Value;

    public EmotionValue(int i)
    {
        switch (i) 
        { 
            case 0:
                Emotion = Emotion.Happiness;
                break;
            case 1:
                Emotion = Emotion.Sadness;
                break;
            case 2:
                Emotion = Emotion.Fear;
                break;
            case 3:
                Emotion = Emotion.Disgust;
                break;
            case 4:
                Emotion = Emotion.Anger;
                break;
            case 5:
                Emotion = Emotion.Surprise;
                break;
            default:
                Emotion = Emotion.Happiness;
                break;
        }
        Value = 0;
    }

    public EmotionValue(int i, float value)
    {
        switch (i)
        {
            case 0:
                Emotion = Emotion.Happiness;
                break;
            case 1:
                Emotion = Emotion.Sadness;
                break;
            case 2:
                Emotion = Emotion.Fear;
                break;
            case 3:
                Emotion = Emotion.Disgust;
                break;
            case 4:
                Emotion = Emotion.Anger;
                break;
            case 5:
                Emotion = Emotion.Surprise;
                break;
            default:
                Emotion = Emotion.Happiness;
                break;
        }
        Value = value;
    }
}