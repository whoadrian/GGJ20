using System;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "GGJ/AudioConfig", fileName = "AudioConfig")]
public class AudioConfig : ScriptableObject {
    public AudioClip[] CircleSound;
    public AudioClip[] TriangleSound;
    public AudioClip[] SquareSound;
    public AudioClip[] PentagonSound;
    public AudioClip[] OctagonSound;
    public AudioClip[] SextagonSound;
    public AudioClip ConnectionSound;

    public AudioClip GetSound(ShapeType shapeType)
    {
        switch (shapeType)
        {
            case ShapeType.CIRCLE:
                return GetSound(CircleSound);
            case ShapeType.TRIANGLE:
                return GetSound(TriangleSound);
            case ShapeType.SQUARE:
                return GetSound(SquareSound);
            case ShapeType.OCTAGON:
                return GetSound(OctagonSound);
            case ShapeType.PENTAGON:
                return GetSound(PentagonSound);
            case ShapeType.SEXTAGON:
                return GetSound(SextagonSound);
            default:
                throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
        }
    }

    private AudioClip GetSound(AudioClip[] clips) {
        return clips[(int)UnityEngine.Random.Range(0, clips.Length)];
    }
}