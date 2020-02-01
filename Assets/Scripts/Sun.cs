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

    public void AddOutgoingId(string id) {
        string[] newList = new string[OutgoingIds.Length + 1];
        for (int i = 0; i < OutgoingIds.Length; i++) {
            newList[i] = OutgoingIds[i];
        }

        newList[newList.Length - 1] = id;
        OutgoingIds = newList;
    }

    public bool RemoveOutgoingId(string id) {
        bool removed = false;
        for (int i = 0; i < OutgoingIds.Length; i++) {
            if (OutgoingIds[i] == id) {
                OutgoingIds[i] = Guid.Empty.ToString();
                removed = true;
            }
        }
        
        RemoveEmptyIds();
        return removed;
    }

    public void RemoveEmptyIds() {
        var validCount = 0;
        for (int i = 0; i < OutgoingIds.Length; i++) {
            if (OutgoingIds[i] != Guid.Empty.ToString()) {
                validCount++;
            }
        }

        if (validCount == OutgoingIds.Length) {
            return;
        }

        string[] newList = new string[validCount];
        var index = 0;
        for (int i = 0; i < OutgoingIds.Length; i++) {
            if (OutgoingIds[i] != Guid.Empty.ToString()) {
                newList[index] = OutgoingIds[i];
                index++;
            }
        }

        OutgoingIds = newList;
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