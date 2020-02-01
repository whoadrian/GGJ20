using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct SunData {
    public string[] OutgoingIds;

    public SunData(string[] outgoingIds) {
        this.OutgoingIds = outgoingIds;
    }
}

public class Sun : MonoBehaviour {
    [ReadOnly, SerializeField, ShowInInspector]
    public SunData Data;

    public void Command(Command c, SequenceState s) {
        // collect shapes
        var shapes = new List<Shape>();
        foreach (var shapeId in Data.OutgoingIds) {
            if (!Shape.TryGetShape(shapeId, out var shape)) {
                Debug.LogError("Sun outgoing ID not found : ID " + shapeId);
                continue;
            }
            
            shapes.Add(shape);
        }
        
        switch (c) {
            case global::Command.NONE:
                break;
            case global::Command.PLAY:
                switch (s) {
                    // if sequence previously stopped, trigger the song
                    case SequenceState.STOPPED:
                        foreach (var shape in shapes) {
                            shape.TriggerShape();
                        }
                        break;
                    case SequenceState.PLAYING:
                        break;
                    case SequenceState.PAUSED:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(s), s, null);
                }
                break;
            case global::Command.PAUSE:
                break;
            case global::Command.STOP:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(c), c, null);
        }

        foreach (var shape in shapes) {
            shape.Command(c);
        }
    }
}