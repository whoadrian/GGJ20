using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelAuthor : MonoBehaviour {

    private GameSystem system;
    
    [Button(ButtonStyle.Box)]
    public void CreateShape(ShapeType type) {
        SanityCheck();

        foreach (var p in system.ShapeAssets.Prefabs) {
            if (p.Type == type) {
                var obj = PrefabUtility.InstantiatePrefab(p.Prefab) as GameObject;
                var shape = obj.GetComponent<Shape>();
                shape.Data = new ShapeData(type);
            }
        }
    }

    [Button(ButtonStyle.Box)]
    public void ConnectShapes() {
        SanityCheck();

        if (UnityEditor.Selection.transforms.Length != 2) {
            Debug.LogError("Please select 2 shapes");
            return;
        }

        var first = (UnityEditor.Selection.transforms[1] == UnityEditor.Selection.activeTransform) ? UnityEditor.Selection.transforms[1] : UnityEditor.Selection.transforms[0];
        var second = (UnityEditor.Selection.transforms[0] == UnityEditor.Selection.activeTransform) ? UnityEditor.Selection.transforms[1] : UnityEditor.Selection.transforms[0];

        var shapeA = first.GetComponent<Shape>();
        var shapeB = second.GetComponent<Shape>();

        if (shapeB.Data.IncomingId != Guid.Empty.ToString()) {
            bool replace = EditorUtility.DisplayDialog("Warning", "Shape B already has an incoming connection. Replace?", "Yes", "No");

            if (!replace) {
                return;
            }
        }

        int shapeAConnectionIndex = -1;
        for (int i = 0; i < (int)shapeA.Data.Type; i++) {
            if (shapeA.Data.OutgoingIds[i] == Guid.Empty.ToString()) {
                shapeAConnectionIndex = i;
            }
        }

        if (shapeAConnectionIndex > -1) {
            shapeB.Data.IncomingId = shapeA.Data.Id;
            shapeA.Data.OutgoingIds[shapeAConnectionIndex] = shapeB.Data.Id;
        }
    }

    [Button(ButtonStyle.Box)]
    public void ClearIncomingConnections() {
        // TODO
    }

    [Button(ButtonStyle.Box)]
    public void ClearOutgoingConnections() {
        // TODO
    }

    [Button(ButtonStyle.Box)]
    public void ClearSunConnection() {
        // TODO
    }

    [Button(ButtonStyle.Box)]
    public void ClearAllSunConnections() {
        // TODO
    }

    [Button(ButtonStyle.Box)]
    public void ConnectShapeToSun() {
        SanityCheck();

        if (UnityEditor.Selection.transforms.Length != 1) {
            Debug.LogError("Please select 1 shape");
            return;
        }

        var shape = UnityEditor.Selection.transforms[0].GetComponent<Shape>();
        if (shape == null) {
            Debug.LogError("Shape not selected");
            return;
        }
        
        system.Sun.Data.OutgoingIds.Add(shape.Data.Id);
    }

    private void SanityCheck() {
        if (system == null) {
            system = GameObject.FindObjectOfType<GameSystem>();
        }
    }
}
