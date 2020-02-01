﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

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
    
    [Header("Assets")]
    public GameObject BallPrefab;
    public LevelConfig LevelConfig;
    public ShapeAssets ShapeAssets;
    
    [Header("Level")]
    public Sequence Sequence;
    public Sun Sun;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Oops, another instance is already there, smth is wrong.");
        }
        
        Instance = this;
    }

    void Start() {
        
    }

    void Update() {
    }

    public void Command(Command c) {
        var sequenceState = Sequence.State;
        if (Sequence.Command(c)) {
            Sun.Command(c, sequenceState);
        }
    }
}