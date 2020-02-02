using System;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "GGJ/AudioConfig", fileName = "AudioConfig")]
public class AudioConfig : ScriptableObject {
    public AudioClip[] CircleSound;
    public float CircleVolume = 1;
    public AudioClip[] TriangleSound;
    public float TriangleVolume = 1;
    public AudioClip[] SquareSound;
    public float SquareVolume = 1;
    public AudioClip[] PentagonSound;
    public float PentagonVolume = 1;
    public AudioClip[] OctagonSound;
    public float OctagonVolume = 1;
    public AudioClip[] SextagonSound;
    public float SextagonVolume = 1;
    public AudioClip ConnectionSound;

    public AudioClip GetSound(ShapeType shapeType, out float volume)
    {
        switch (shapeType)
        {
            case ShapeType.CIRCLE:
                volume = CircleVolume;
                return GetSound(CircleSound);
            case ShapeType.TRIANGLE:
                volume = TriangleVolume;
                return GetSound(TriangleSound);
            case ShapeType.SQUARE:
                volume = SquareVolume;
                return GetSound(SquareSound);
            case ShapeType.OCTAGON:
                volume = OctagonVolume;
                return GetSound(OctagonSound);
            case ShapeType.PENTAGON:
                volume = PentagonVolume;
                return GetSound(PentagonSound);
            case ShapeType.SEXTAGON:
                volume = SextagonVolume;
                return GetSound(SextagonSound);
            default:
                throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
        }
    }

    private AudioClip GetSound(AudioClip[] clips) {
        return clips[(int)UnityEngine.Random.Range(0, clips.Length)];
    }
}