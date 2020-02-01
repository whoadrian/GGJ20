using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelAuthor : MonoBehaviour {
    private static GameSystem system;

    [Button(ButtonStyle.Box)]
    public static void CreateShape(Vector2 pos, ShapeType type = ShapeType.CIRCLE) {
        SanityCheck();

        var shapeParent = GameObject.Find("SHAPES");
        if (shapeParent == null) {
            shapeParent = new GameObject("SHAPES");
            shapeParent.transform.position = Vector3.zero;
        }
        
#if UNITY_EDITOR
        var obj = PrefabUtility.InstantiatePrefab(GameSystem.Instance.ShapePrefab, shapeParent.transform) as GameObject;
#else
        var obj = GameObject.Instantiate(GameSystem.Instance.ShapePrefab, shapeParent.transform);
#endif

        var shape = obj.GetComponent<Shape>();
        shape.Data = new ShapeData(type);
        shape.transform.position = pos;

        ValidateConnections();
    }

    [BoxGroup("Shape Connections")]
    [Button(ButtonStyle.Box)]
    public void ConnectShapes() {
        SanityCheck();
        if (GetPairShape(out var shapeA, out var shapeB)) {
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
#if UNITY_EDITOR
                EditorUtility.SetDirty(shapeB);
                EditorUtility.SetDirty(shapeA);
#endif
                ValidateConnections();
            }
        }
    }

    [BoxGroup("Shape Connections")]
    [Button(ButtonStyle.Box)]
    public void ClearIncomingConnections() {
        SanityCheck();
        if (GetSingleShape(out var shape)) {
            shape.Data.IncomingId = Guid.Empty.ToString();
            EditorUtility.SetDirty(shape);
            ValidateConnections();
        }
    }

    [BoxGroup("Shape Connections")]
    [Button(ButtonStyle.Box)]
    public void ClearOutgoingConnections() {
        SanityCheck();
        if (GetSingleShape(out var shape)) {
            for (int i = 0; i < shape.Data.OutgoingIds.Length; i++) {
                shape.Data.OutgoingIds[i] = Guid.Empty.ToString();
                EditorUtility.SetDirty(shape);
                ValidateConnections();
            }
        }
    }

    [BoxGroup("Sun Connections")]
    [Button(ButtonStyle.Box)]
    public void ConnectShapeToSun() {
        SanityCheck();

        if (GetSingleShape(out var shape)) {
            system.Sun.Data.AddOutgoingId(shape.Data.Id);
            EditorUtility.SetDirty(shape);
            ValidateConnections();
        }
    }

    [BoxGroup("Sun Connections")]
    [Button(ButtonStyle.Box)]
    public void ClearSunConnection() {
        SanityCheck();
        if (GetSingleShape(out var shape)) {
            system.Sun.Data.RemoveOutgoingId(shape.Data.Id);
            EditorUtility.SetDirty(shape);
            ValidateConnections();
        }
    }

    [BoxGroup("Sun Connections")]
    [Button(ButtonStyle.Box)]
    public void ClearAllSunConnections() {
        SanityCheck();
        system.Sun.Data.OutgoingIds = new string[0];
        EditorUtility.SetDirty(system.Sun);
        ValidateConnections();
    }

    [BoxGroup("Validation")]
    [Button(ButtonStyle.Box)]
    public static void ValidateConnections() {
        SanityCheck();
        var allShapes = GameObject.FindObjectsOfType<Shape>();

        if (system == null || system.Sun == null) {
            return;
        }

        Debug.Log("Validating Connections...");

        // Check for duplicates in sun's connections
        for (int i = 0; i < system.Sun.Data.OutgoingIds.Length - 1; i++) {
            var idA = system.Sun.Data.OutgoingIds[i];
            if (idA == Guid.Empty.ToString()) {
                continue;
            }

            for (int j = i + 1; j < system.Sun.Data.OutgoingIds.Length; j++) {
                var idB = system.Sun.Data.OutgoingIds[j];
                if (idB == Guid.Empty.ToString()) {
                    continue;
                }

                if (idA == idB) {
                    system.Sun.Data.OutgoingIds[j] = Guid.Empty.ToString();
                    EditorUtility.SetDirty(system.Sun);
                }
            }
        }

        // Remove sun's empty connections
        system.Sun.Data.RemoveEmptyIds();
        EditorUtility.SetDirty(system.Sun);

        // Check if all sun's outgoing ids have existing shapes, remove if so
        for (int i = 0; i < system.Sun.Data.OutgoingIds.Length; i++) {
            var id = system.Sun.Data.OutgoingIds[i];
            if (id == Guid.Empty.ToString()) {
                continue;
            }

            bool valid = false;
            foreach (var shape in allShapes) {
                if (shape.Data.Id == id) {
                    valid = true;
                    break;
                }
            }

            if (!valid) {
                system.Sun.Data.RemoveOutgoingId(id);
                EditorUtility.SetDirty(system.Sun);
            }
        }

        // Go through all shapes and remove outgoing id duplicates
        foreach (var shape in allShapes) {
            for (int i = 0; i < shape.Data.OutgoingIds.Length - 1; i++) {
                var idA = shape.Data.OutgoingIds[i];
                if (idA == Guid.Empty.ToString()) {
                    continue;
                }

                for (int j = i + 1; j < shape.Data.OutgoingIds.Length; j++) {
                    var idB = shape.Data.OutgoingIds[j];
                    if (idB == Guid.Empty.ToString()) {
                        continue;
                    }

                    if (idA == idB) {
                        shape.Data.OutgoingIds[j] = Guid.Empty.ToString();
                        EditorUtility.SetDirty(shape);
                    }
                }
            }
        }

        // Go through all shapes' connections and check if any shapes are missing. Remove ids if so
        foreach (var shape in allShapes) {
            var inId = shape.Data.IncomingId;
            if (inId != Guid.Empty.ToString()) {
                bool valid = false;
                foreach (var s in allShapes) {
                    if (s.Data.Id == inId) {
                        valid = true;
                        break;
                    }
                }

                if (!valid) {
                    shape.Data.IncomingId = Guid.Empty.ToString();
                    EditorUtility.SetDirty(shape);
                }
            }

            for (int i = 0; i < shape.Data.OutgoingIds.Length; i++) {
                var outId = shape.Data.OutgoingIds[i];
                if (outId == Guid.Empty.ToString()) {
                    continue;
                }

                bool valid = false;
                foreach (var s in allShapes) {
                    if (s.Data.Id == outId) {
                        valid = true;
                        break;
                    }
                }

                if (!valid) {
                    shape.Data.OutgoingIds[i] = Guid.Empty.ToString();
                    EditorUtility.SetDirty(shape);
                }
            }
        }
    }

    // ----------------------------------------------------------------

    private static void SanityCheck() {
        if (system == null) {
            system = GameSystem.Instance != null ? GameSystem.Instance : FindObjectOfType<GameSystem>();
        }
    }

    private static bool GetSingleShape(out Shape shape) {
        shape = null;

        if (UnityEditor.Selection.transforms.Length != 1) {
            Debug.LogError("Please select 1 shape");
            return false;
        }

        shape = UnityEditor.Selection.transforms[0].GetComponent<Shape>();
        if (shape == null) {
            Debug.LogError("Shape not selected");
            return false;
        }

        return true;
    }

    private static bool GetPairShape(out Shape shapeA, out Shape shapeB) {
        shapeA = null;
        shapeB = null;

        if (Selection.transforms.Length != 2) {
            Debug.LogError("Please select 2 shapes");
            return false;
        }

        var first = (Selection.transforms[1] == Selection.activeTransform) ? Selection.transforms[1] : Selection.transforms[0];
        var second = (Selection.transforms[0] == Selection.activeTransform) ? Selection.transforms[1] : Selection.transforms[0];

        shapeA = first.GetComponent<Shape>();
        shapeB = second.GetComponent<Shape>();

        return shapeA != null && shapeB != null;
    }

    public static void SafeDestroy(GameObject obj) {
        if (Application.isPlaying) {
            GameObject.Destroy(obj);
        } else {
            GameObject.DestroyImmediate(obj);
        }
    }
}