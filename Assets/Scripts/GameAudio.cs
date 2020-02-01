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