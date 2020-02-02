using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using whoa.UX;

public enum Command {
    NONE,
    PLAY,
    PAUSE,
    STOP
}

public class GameSystem : MonoBehaviour {
    public static GameSystem Instance;

    [Header("General Settings")]
    public float BallSpeed = 0.5f;
    public float MaxBallSpeed = 3.0f;
    public float MinBallSpeed = 0.2f;
    public float MaxDuration = 30;
    public float MinDuration = 2;
    
    [Header("Assets")]
    public GameObject BallPrefab;
    public GameObject ShapePrefab;
    public LevelConfig LevelConfig;
    public InteractableGizmoConfig GizmoConfig;

    [Header("Level")]
    public Camera Cam;
    public Sequence Sequence;
    public Sun Sun;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Oops, another instance is already there, smth is wrong.");
        }
        
        Instance = this;
    }

    void Start() {
        Sequence.OnSequenceRestart += OnSequenceRestart;
    }

    private void OnSequenceRestart() {
        Debug.Log("Sequence restart");
        Sun.Command(global::Command.STOP, false);
        KillRemainingBalls();
        Sun.Command(global::Command.PLAY, true);
    }

    void Update() {
    }

    public void Command(Command c) {
        var sequenceState = Sequence.State;
        if (Sequence.Command(c)) {
            Sun.Command(c, sequenceState == SequenceState.STOPPED);
            if (c == global::Command.STOP) {
                KillRemainingBalls();
            }
        }
    }

    private void KillRemainingBalls() {
        var balls = GameObject.FindObjectsOfType<Ball>();
        foreach (var b in balls) {
            if (b != null) {
                LevelAuthor.SafeDestroy(b.gameObject);
            }
        }
    }
}