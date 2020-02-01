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
    //[ReadOnly, ShowInInspector]
    public SequenceData Data;

    private SequenceState state = SequenceState.STOPPED;
    public SequenceState State {
        get { return state; }
    }


    public Action OnSequenceRestart;

    private float startTime = 0;
    public float StartTime => startTime;
    private float currentTime = 0;
    public float CurrentTime => currentTime;
    private Coroutine loop;

    private void Start() => Command(global::Command.STOP);

    void Update() {
        if (State != SequenceState.PAUSED && State != SequenceState.STOPPED) {
            currentTime += Time.deltaTime;
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
                newState = SequenceState.PAUSED;
                break;
            case global::Command.STOP:
                newState = SequenceState.STOPPED;
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
                    currentTime = startTime;
                }
                break;
            case SequenceState.PLAYING:
                if (state == SequenceState.PAUSED)
                    break;
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
        startTime = currentTime;

        while (currentTime - startTime <= Data.duration) {
            yield return null;
        }
        
        OnSequenceRestart?.Invoke();

        loop = StartCoroutine(Loop());
    }
}