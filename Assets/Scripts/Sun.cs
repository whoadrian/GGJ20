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

    public void Command(Command c, bool triggerFirstConnections) {
        // collect shapes
        var shapes = new List<Shape>();
        foreach (var shapeId in Data.OutgoingIds) {
            if (!Shape.TryGetShape(shapeId, out var shape)) {
                Debug.LogError("Sun outgoing ID not found : ID " + shapeId);
                continue;
            }
            
            shapes.Add(shape);
        }

        if (triggerFirstConnections) {
            foreach (var shape in shapes) {
                shape.TriggerShape();
            }
        }

        foreach (var shape in shapes) {
            shape.Command(c);
        }
    }
}