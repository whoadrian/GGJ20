using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct SequenceData {
    public float duration;
}

public enum SequenceState {
    STOPPED,
    PLAYING,
    PAUSED
}

public class Sequence : MonoBehaviour {
    [ReadOnly, ShowInInspector]
    public SequenceData Data;

    private SequenceState state;
    public SequenceState State {
        get { return state; }
    }

    private float startTime = 0;
    private float time = 0;
    private Coroutine loop;
    
    void Start() {
    }

    void Update() {
        if (State != SequenceState.PAUSED) {
            time += Time.deltaTime;
        }
    }

    public bool Command(Command c) {
        SequenceState newState = SequenceState.PAUSED;
        switch (c) {
            case global::Command.NONE:
                return false;
            case global::Command.PLAY:
                newState = SequenceState.PLAYING;
                break;
            case global::Command.PAUSE:
                newState = SequenceState.PLAYING;
                break;
            case global::Command.STOP:
                newState = SequenceState.PLAYING;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(c), c, null);
        }
        
        if (newState == state) {
            return false;
        }

        switch (newState) {
            case SequenceState.STOPPED:
                if (loop != null) {
                    StopCoroutine(loop);
                }
                break;
            case SequenceState.PLAYING:
                loop = StartCoroutine(Loop());
                break;
            case SequenceState.PAUSED:
                if (loop != null) {
                    StopCoroutine(loop);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        state = newState;
        return true;
    }

    private IEnumerator Loop() {
        startTime = time;

        while (time - startTime <= Data.duration) {
            yield return null;
        }

        loop = StartCoroutine(Loop());
    }
}