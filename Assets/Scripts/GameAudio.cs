using System;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance;
    public AudioConfig Config;

    private void Awake() {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }
}


[CreateAssetMenu(menuName = "GGJ/AudioConfig", fileName = "AudioConfig")]
public class AudioConfig : ScriptableObject {
    public AudioClip CircleSound;
    public AudioClip TriangleSound;
    public AudioClip SquareSound;
    public AudioClip ConnectionSound;

    public AudioClip GetSound(ShapeType shapeType)
    {
        switch (shapeType)
        {
            case ShapeType.CIRCLE:
                return CircleSound;
            case ShapeType.TRIANGLE:
                return TriangleSound;
            case ShapeType.SQUARE:
                return SquareSound;
            default:
                throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
        }
    }
}